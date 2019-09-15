using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace QueueFunction
{
    public static class QueueFunction
    {
        [FunctionName("QueueFunction")]
        public static void Run(
            [QueueTrigger("test-input", Connection = "AzureWebJobsStorage")]string myQueueItem, 
            [Queue("test-output", Connection = "AzureWebJobsStorage")] out string myQueueResult,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            myQueueResult = myQueueItem + " ===> Queue Result";
        }
    }
}
