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
using Microsoft.Azure.WebJobs.Host;

namespace Planr.Tasks
{
    public static class GetDays
    {
        [FunctionName("GetDays")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "day")]HttpRequest req,
                        ClaimsPrincipal principal,
            [CosmosDB(
                databaseName: "Tasks",
                collectionName: "Collection1",
                ConnectionStringSetting = "COSMOSDB_CONNECTION")]  DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var claims = principal.Claims;
            string userId = "";
            log.LogInformation($"name: {principal.Identity.Name}");
            foreach (var innerClaim in claims)
            {
                log.LogInformation($"Claim: {innerClaim.Type}, Value: {innerClaim.Value}");
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
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("tasks", "Collection1");
            IDocumentQuery<Day> query = client.CreateDocumentQuery<Day>(collectionUri, option)
                .Where(p => p.UserId == userId)
                .AsDocumentQuery();
            List<Day> days = new List<Day>();
            while (query.HasMoreResults)
            {
                foreach (Day result in await query.ExecuteNextAsync())
                {
                    days.Add(result);
                }
            }
            return new OkObjectResult(days);
        }
    }
}


