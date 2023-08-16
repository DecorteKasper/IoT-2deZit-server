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
using IoT_2deZit_server.Models;

namespace MCT.Function
{
    public static class CreateEvent
    {
        [FunctionName("CreateEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events/{EventId}")] HttpRequest req,
            ILogger log, string EventId)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");

                // Initialize CosmosClient
                using (var cosmosClient = new CosmosClient(connectionString))
                {
                    var databaseName = "IoT-examen2deZit";
                    var containerName = "events";

                    var database = cosmosClient.GetDatabase(databaseName);
                    var container = database.GetContainer(containerName);

                    var eventObject = JsonConvert.DeserializeObject<EventDetails>(body);

                    var response = await container.CreateItemAsync(eventObject, new PartitionKey(eventObject.EventId));
                }

                return new OkObjectResult("Event created successfully");


            }
            catch (System.Exception ex)
            {
                log.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
