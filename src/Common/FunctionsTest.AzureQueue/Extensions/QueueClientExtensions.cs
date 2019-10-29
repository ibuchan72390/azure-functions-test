using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionsTest.AzureQueue.Extensions
{
    public static class QueueClientExtensions
    {
        #region Core Reusable Functionality

        /*
         * The core functionality that makes all of this work...
         * We'll create specific extensions for each of the Request / Response implementations.
         */

        public static async Task<CloudQueueMessage> AddRequestAndPollAsync(
            this CloudQueueClient client,
            string inputQueueName,
            string outputQueueName,
            int peekCountOverload = 10)
        {
            return await client.AddMessageAndPollAsync<string>(null, inputQueueName, outputQueueName);
        }

        public static async Task AddTypedMessage<TRequest>(
            this CloudQueueClient client,
            TRequest request,
            string inputQueueName)
        {

            CloudQueue inputQueue = client.GetQueueReference(inputQueueName);
            await inputQueue.CreateIfNotExistsAsync();
            var message = JsonConvert.SerializeObject(request);
            await inputQueue.AddMessageAsync(new CloudQueueMessage(message));
        }

        public static async Task<CloudQueueMessage> AddMessageAndPollAsync<TRequest>(
            this CloudQueueClient client,
            TRequest request,
            //Guid guid,
            string inputQueueName,
            string outputQueueName,
            int peekCountOverload = 10,
            // We peek like fucking crazy, this might be slowing us down here
            int peekThrottle = 100
        )
        {
            // Should probably log this somehow
            Guid requestGuid = Guid.NewGuid();

            CloudQueue inputQueue = client.GetQueueReference(inputQueueName);
            await inputQueue.CreateIfNotExistsAsync();

            CloudQueue outputQueue = client.GetQueueReference(outputQueueName);
            await outputQueue.CreateIfNotExistsAsync();

            #region Polling Process (Could we somehow use the Polly process here?)

            string queueMessage;
            if (request == null)
            {
                queueMessage = requestGuid.ToString();
            }
            else
            {
                queueMessage = request.ToQueueMessage(requestGuid);
            }

            CloudQueueMessage inputMessage = new CloudQueueMessage(queueMessage);
            await inputQueue.AddMessageAsync(inputMessage);

            string stringId = requestGuid.ToString();
            CloudQueueMessage result = null;
            bool done = false;

            while (result == null || !done)
            {
                /*
                 * This is not sufficient simply because if we get one message stuck at the top of the queue without a recipient, the entire chain fails.
                 * We need a way of moving beyond the top item on the queue and checking the other results.
                 */
                var msgs = await outputQueue.PeekMessagesAsync(peekCountOverload);
                var msgDictionary = msgs.ToDictionary(x => x.AsString.GetQueueMessageId(), x => x);

                if (msgDictionary.ContainsKey(stringId))
                {
                    var results = await outputQueue.GetMessagesAsync(peekCountOverload);
                    result = results.ToDictionary(x => x.AsString.GetQueueMessageId(), x => x)[stringId];

                    if (result.AsString.GetQueueMessageId() == stringId)
                    {
                        done = true;
                    }
                }

                Thread.Sleep(peekThrottle);
            }

            await outputQueue.DeleteMessageAsync(result);

            #endregion

            return result;
        }

        public static async Task<TResponse> AddMessageAndPollAsync<TRequest, TResponse>(
            this CloudQueueClient client,
            TRequest request,
            string inputQueueName,
            string ouptputQueueName,
            int peekCountOverload = 10
        )
        {
            var result = await client.AddMessageAndPollAsync(request, inputQueueName, ouptputQueueName, peekCountOverload);

            return JsonConvert.DeserializeObject<TResponse>(result.AsString.GetQueueMessage());
        }

        public static async Task AddMessageAndPollVoidAsync<TRequest>(
            this CloudQueueClient client,
            TRequest request,
            string inputQueueName,
            string ouptputQueueName,
            int peekCountOverload = 10)
        {
            await client.AddMessageAndPollAsync(request, inputQueueName, ouptputQueueName, peekCountOverload);
        }

        public static async Task<TResponse> AddRequestAndPollAsync<TResponse>(
            this CloudQueueClient client,
            string inputQueueName,
            string ouptputQueueName,
            int peekCountOverload = 10)
        {
            var result = await client.AddRequestAndPollAsync(inputQueueName, ouptputQueueName, peekCountOverload);

            return JsonConvert.DeserializeObject<TResponse>(result.AsString.GetQueueMessage());
        }

        #endregion
    }
}
