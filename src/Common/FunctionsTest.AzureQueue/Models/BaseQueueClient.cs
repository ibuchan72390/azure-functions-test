using Microsoft.Azure.Storage.Queue;

namespace FunctionsTest.AzureQueue.Models
{
    public abstract class BaseQueueClient
    {
        protected CloudQueueClient Client;

        /*
         * I like this idea for tracking purposes, but it's wildly limited.
         * What if two submissions are made to the same queue?  The results could get confused
         * Unique guids must be made for each request JIC
         */
        //private Guid _guid;

        protected BaseQueueClient(CloudQueueClient client)
        {
            Client = client;
        }
    }
}
