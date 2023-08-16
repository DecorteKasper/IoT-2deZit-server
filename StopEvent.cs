using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace MCT.Function
{
    public static class StopEvent
    {
        [FunctionName("StopEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "stopevent/{EventId}")] HttpRequest req,
            ILogger log, int EventId)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");

                using (var cosmosClient = new CosmosClient(connectionString))
                {
                    var databaseName = "IoT-examen2deZit";
                    var containerName = "events";
                    
                    var database = cosmosClient.GetDatabase(databaseName);
                    var container = database.GetContainer(containerName);



                    var stopTimeObject = new
                    {
                        EventId = EventId,
                        StopTime = DateTime.Now
                    };

                    var response = await container.CreateItemAsync(stopTimeObject, new PartitionKey(EventId));
                }

                return new OkObjectResult("Event gestopt!");
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
