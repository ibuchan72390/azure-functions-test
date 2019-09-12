using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace QueueFunction
{
    public static class QueueFunction
    {
        [FunctionName("QueueFunction")]
        public static void Run(
            [QueueTrigger("test-input", Connection = "QueueStorage")]string myQueueItem, 
            [Queue("test-output", Connection = "QueueStorage")] out string myQueueResult,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            myQueueResult = $"Result: {myQueueItem}";
        }
    }
}
