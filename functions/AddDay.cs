using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Planr.Tasks
{
    public static class AddDay
    {
        [FunctionName("AddDay")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ClaimsPrincipal principal,
            [CosmosDB(
                databaseName: "tasks",
                collectionName: "Collection1",
                ConnectionStringSetting = "COSMOSDB_CONNECTION",
                CreateIfNotExists = true)] out Day day,
            [CosmosDB(
                databaseName: "tasks",
                collectionName: "Collection1",
                ConnectionStringSetting = "COSMOSDB_CONNECTION")] DocumentClient client,       
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger processing add day");
            if (principal != null)
            {
            string json = req.ReadAsStringAsync().Result;
            var taskJson = JsonConvert.DeserializeObject<Day>(json);
              var claims = principal.Claims;
                 log.LogInformation($"name: {principal.Identity.Name}");               
                foreach(var innerClaim in claims){
                    log.LogInformation($"Claim: {innerClaim.Type}, Value: {innerClaim.Value}");   
                    if (innerClaim.Type == ClaimTypes.NameIdentifier && innerClaim.Value != null)
                    {
                            log.LogInformation($"User ID: {innerClaim.Value}");   
                            taskJson.UserId = innerClaim.Value;
                    }
                }
                if(taskJson.UserId.Length == 0){
                    day = null;
                    return new BadRequestResult();
                }
            taskJson.Date = taskJson.Date.AddHours(-taskJson.Date.Hour).AddMinutes(-taskJson.Date.Minute).AddSeconds(-taskJson.Date.Second);
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("tasks", "Collection1");
            var queryString = $"SELECT * FROM Day WHERE Day.userId = \"{taskJson.UserId}\" AND Day.date = \"{taskJson.Date.ToString("s")}Z\"";
            IQueryable<Day> query = client.CreateDocumentQuery<Day>(collectionUri, queryString, option);
            List<Day> days = new List<Day>();
            foreach (Day result in query)
            {
                result.Tasks = result.Tasks.OrderBy(c => c.Priority).ToList();
                days.Add(result);
            }
            if(days.Count > 0){
                taskJson.Id = days.First().Id;
            }
            
            day = taskJson;
            return new OkResult();
        }
            else
            {
                day = null;
                return new UnauthorizedResult();
            }
    }
}
}
