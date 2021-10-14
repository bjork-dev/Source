using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using AzureFunctions.Models;

namespace Calculator.Functions
{
    public static class AddNumbers // api/AddNumbers
    {
        [FunctionName("AddNumbers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "Calculations",
                collectionName: "Items",
                ConnectionStringSetting = "CosmosDbConnectionString")]
                IAsyncCollector<dynamic> calculation,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string data = JsonConvert.DeserializeObject(requestBody).ToString();

            if (!data.Contains('+'))
                return new BadRequestObjectResult("Missing '+' operator");

            try
            {
                DataTable dt = new DataTable();
                string result = dt.Compute(data, "").ToString();
                log.LogInformation(result);
                await calculation.AddAsync(new Calculation
                {
                    Id = System.Guid.NewGuid().ToString(),
                    Result = $"{data} = {result}",
                    RunDate = DateTime.Now
                });

                return new OkObjectResult(result);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
