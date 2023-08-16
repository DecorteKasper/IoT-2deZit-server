using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System;
using IoT_2deZit_server.Models;
using Microsoft.Azure.Cosmos;

namespace MCT.Function
{
    public class IoTReadMessage
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("IoTReadMessage")]
        public async void Run([IoTHubTrigger("messages/events", Connection = "IoT_Hub")] EventData message, ILogger log)
        {
            var ConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");

            try
            {
                var json = Encoding.UTF8.GetString(message.Body.Array);
                EventRecord eventMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<EventRecord>(json);

                if (eventMessage.DeviceId = "kasperdecorte")
                {
                    CosmosClientOptions options = new CosmosClientOptions()
                    {
                        ConnectionMode = ConnectionMode.Gateway
                    };
                    CosmosClient client = new CosmosClient(ConnectionString, options);
                    var container = client.GetContainer("kasperdct-cosmos", "events");
                    eventMessage.Id = Guid.NewGuid();
                    await container.ReplaceItemAsync<EventRecord>(eventMessage, eventMessage.EventId);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

        }
    }
}