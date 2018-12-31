using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading;
using System.Collections.Generic;

namespace Planr.Tasks
{
    public static class GetUser
    {
        [FunctionName("GetUser")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ClaimsPrincipal principal,
            ILogger log)
        {
            log.LogInformation("Processing get user request");
            if (principal != null)
            {
                var claims = principal.Claims;
                 log.LogInformation($"name: {principal.Identity.Name}");               
                var claimsList = new Dictionary<string, string>();
                foreach(var innerClaim in claims){
                    claimsList.Add(innerClaim.Type, innerClaim.Value);
                    if (innerClaim.Type == ClaimTypes.NameIdentifier && innerClaim.Value != null)
                    {
                            log.LogInformation($"UserId: {innerClaim.Value}") ;
                    }
                }

                return new OkObjectResult(claimsList);
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}
