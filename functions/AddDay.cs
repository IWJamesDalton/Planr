using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
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
                foreach (var innerClaim in claims)
                {
                    if (innerClaim.Type == ClaimTypes.NameIdentifier && innerClaim.Value != null)
                    {
                        taskJson.UserId = innerClaim.Value;
                    }
                }
                if (taskJson.UserId.Length == 0)
                {
                    day = null;
                    return new BadRequestResult();
                }
                taskJson.Date = taskJson.Date.AddHours(-taskJson.Date.Hour).AddMinutes(-taskJson.Date.Minute).AddSeconds(-taskJson.Date.Second);
                var option = new FeedOptions { EnableCrossPartitionQuery = true };
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("tasks", "Collection1");
                var queryString = $"SELECT * FROM Day WHERE Day.userId = \"{taskJson.UserId}\" AND Day.date = \"{taskJson.Date.ToString("s")}Z\"";

                foreach (Document document in client.CreateDocumentQuery(
            collectionUri,
            queryString,
            option))
                {
                    log.LogInformation($"Self Link: {document.SelfLink}");
                    Day dayDocument = (Day)(dynamic)document;
                    RequestOptions options = new RequestOptions {
                        PartitionKey = new PartitionKey(dayDocument.ProductivityScore)
                    };
                    var result = client.DeleteDocumentAsync(document.SelfLink, options).Result;
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
