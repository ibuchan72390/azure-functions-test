using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServiceBusFunction
{
    public static class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public static void Run(
            [ServiceBusTrigger("test-output", Connection = "QueueStorage")] string myQueueItem, 
            [ServiceBus("service-bus-result", Connection = "QueueStorage")] out string myQueueResult,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            myQueueResult = myQueueItem + "--- Service Bus Result";
        }
    }
}
