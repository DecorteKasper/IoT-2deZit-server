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
    public static class StartEvent
    {
        [FunctionName("StartEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "startevent/{EventId}")] HttpRequest req,
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


                    var startTimeObject = new
                    {
                        EventId = EventId,
                        StartTime = DateTime.Now
                    };

                    // Store the start time in Cosmos DB
                    var response = await container.CreateItemAsync(startTimeObject, new PartitionKey(EventId));
                }

                return new OkObjectResult("Event gestart!");
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}