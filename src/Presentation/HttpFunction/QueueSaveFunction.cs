using Microsoft.Azure.WebJobs;
using Microsoft.Build.Framework;
using System;
using System.Threading.Tasks;

namespace HttpFunction
{
    public static class QueueSaveFunction
    {
        /*
         * Can we do any crazy initialization logic up here?  
         * We already have had to grab CloudStorageAccount a couple times...
         * Is there some way we can containerize some of this into our static environment here?
         */

        [FunctionName("QueueFunction")]
        public static async Task Run(
            [QueueTrigger("test-input", Connection = "AzureWebJobsStorage")]string myQueueItem,
            [Queue("test-output", Connection = "AzureWebJobsStorage")] out string myQueueResult,
            ILogger log)
        {
            string webJobStorage = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            // Setup our StorageAccount
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(webJobStorage);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to the required containers
            CloudQueue inputQueue = queueClient.GetQueueReference("test-input");
            await inputQueue.CreateIfNotExistsAsync();

            CloudQueue outputQueue = queueClient.GetQueueReference("test-output");
            await outputQueue.CreateIfNotExistsAsync();

            string id = context.InvocationId.ToString();

            CloudQueueMessage inputMessage = new CloudQueueMessage($"{id}{delimiter}{name}", false);
            await inputQueue.AddMessageAsync(inputMessage);
        }
    }
}
