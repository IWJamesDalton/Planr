using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Net;
using System.Linq;
using System.Globalization;
using Microsoft.Azure.WebJobs.Host;

namespace Planr.Tasks
{
    public static class GetDay
    {
        [FunctionName("GetDay")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "day/date")]HttpRequest req,
            ClaimsPrincipal principal,
            [CosmosDB(
                databaseName: "tasks",
                collectionName: "Collection1",
                ConnectionStringSetting = "COSMOSDB_CONNECTION")]  DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var claims = principal.Claims;
            string userId = "";
            foreach (var innerClaim in claims)
            {
                if (innerClaim.Type == ClaimTypes.NameIdentifier && innerClaim.Value != null)
                {
                    log.LogInformation($"User ID: {innerClaim.Value}");
                    userId = innerClaim.Value;
                }
            }
            if (userId.Length == 0)
            {
                return new BadRequestResult();
            }
            
            var queryDate = DateTime.Parse(req.Query["date"], new CultureInfo("en-GB"));
            queryDate = queryDate.AddHours(-queryDate.Hour).AddMinutes(-queryDate.Minute).AddSeconds(-queryDate.Second);
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("tasks", "Collection1");
            var queryString = $"SELECT * FROM Day WHERE Day.userId = \"{userId}\" AND Day.date = \"{queryDate.ToString("s")}Z\"";
            IQueryable<Day> query = client.CreateDocumentQuery<Day>(collectionUri, queryString, option);
            List<Day> days = new List<Day>();
            foreach (Day result in query)
            {
                result.Tasks = result.Tasks.OrderBy(c => c.Priority).ToList();
                days.Add(result);
            }
            if(days.Count == 0){
                return new NoContentResult();
            }
            return new OkObjectResult(days);
        }
    }
}
