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
using System.Collections.Generic;
using IoT_2deZit_server.Models;

namespace MCT.Function
{
    public static class GetEventDetails
    {
        [FunctionName("GetEventDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events/{EventId}")] HttpRequest req,
            ILogger log, string EventId)
        {
            var ConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");

            try
            {
                CosmosClientOptions options = new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Gateway
                };

                CosmosClient client = new CosmosClient(ConnectionString, options);

                var container = client.GetContainer("IoT-examen2deZit", "events");
                var sqlQueryText = "SELECT * FROM c WHERE c.EventId = @EventId";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@EventId", EventId);
                var iterator = container.GetItemQueryIterator<EventDetails>(queryDefinition);
                var results = new List<EventDetails>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return new OkObjectResult(results);

            }
            catch (System.Exception ex)
            {
                log.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
