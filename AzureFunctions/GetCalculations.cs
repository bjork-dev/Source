using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Calculator.Functions
{
    public static class GetCalculations
    {
        [FunctionName("GetCalculations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "Calculations",
                collectionName: "Items",
                ConnectionStringSetting = "CosmosDbConnectionString")]
                string[] calcs,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            if (calcs == null)
                return new NotFoundObjectResult("No entries found.");
            return new OkObjectResult(calcs);
        }
    }
}
