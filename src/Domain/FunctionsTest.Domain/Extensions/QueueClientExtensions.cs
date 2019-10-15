using FunctionsTest.Domain.Models.Constants;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionsTest.Domain.Extensions
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

        public static ApplicationQueueClient GetApplicationQueueClient(
            this CloudQueueClient client)
        {
            return new ApplicationQueueClient(client);
        }

        public static PersistenceQueueClient GetPersistenceQueueClient(
            this CloudQueueClient client)
        {
            return new PersistenceQueueClient(client);
        }
    }

    /*
     * This will be a uniform place for us to locate our queue wrappers
     * This will help us wrap up the complexity and annoyances of working with the queues
     */
    public class ApplicationQueueClient : BaseQueueClient
    {
        public ApplicationQueueClient(CloudQueueClient client)
            :base(client)
        {
        }

        public async Task<Domain.Models.Application.CreatePersonResponse> CreatePerson(
            Domain.Models.Application.CreatePersonCommand command)
        {
            return await Client.AddMessageAndPollAsync<
                Domain.Models.Application.CreatePersonCommand,
                Domain.Models.Application.CreatePersonResponse>(
                command,
                QueueConstants.Application.Person.CreateEntity.InputQueue,
                QueueConstants.Application.Person.CreateEntity.OutputQueue
            );
        }

        public async Task<Domain.Models.Application.UpdatePersonResponse> UpdatePerson(
            Domain.Models.Application.UpdatePersonCommand command)
        {
            return await Client.AddMessageAndPollAsync<
                Domain.Models.Application.UpdatePersonCommand,
                Domain.Models.Application.UpdatePersonResponse>(
                command,
                QueueConstants.Application.Person.UpdateEntity.InputQueue,
                QueueConstants.Application.Person.UpdateEntity.OutputQueue
            );
        }

        public async Task<Domain.Models.Application.GetPersonResponse> GetPerson(
            Domain.Models.Application.GetPersonQuery query)
        {
            return await Client.AddMessageAndPollAsync<
                Domain.Models.Application.GetPersonQuery,
                Domain.Models.Application.GetPersonResponse>(
                query,
                QueueConstants.Application.Person.GetEntity.InputQueue,
                QueueConstants.Application.Person.GetEntity.OutputQueue
            );
        }

        public async Task<Domain.Models.Application.GetPeopleResponse> GetPeople()
        {
            return await Client.AddRequestAndPollAsync<
                Domain.Models.Application.GetPeopleResponse>(
                QueueConstants.Application.Person.GetEntities.InputQueue,
                QueueConstants.Application.Person.GetEntities.OutputQueue
            );
        }

        public async Task<Domain.Models.Application.DeletePersonResponse> DeletePerson(
            Domain.Models.Application.DeletePersonCommand command)
        {
            return await Client.AddMessageAndPollAsync<
                Domain.Models.Application.DeletePersonCommand,
                Domain.Models.Application.DeletePersonResponse>(
                command,
                QueueConstants.Application.Person.DeleteEntity.InputQueue,
                QueueConstants.Application.Person.DeleteEntity.OutputQueue);
        }
    }


    public class PersistenceQueueClient : BaseQueueClient
    {
        public PersistenceQueueClient(
            CloudQueueClient client)
            :base(client)
        {
        }

        /*
         * I don't really see the point of both insert and updates...
         * When the string is null it's just considered a new object.
         */
        public async Task<Domain.Models.Persistence.Person> CreatePerson(
            Domain.Models.Persistence.Person command)
        {
            return await Client.AddMessageAndPollAsync<
                Domain.Models.Persistence.Person,
                Domain.Models.Persistence.Person>(
                command,
                QueueConstants.Persistence.Person.CreateEntity.InputQueue,
                QueueConstants.Persistence.Person.CreateEntity.OutputQueue
            );
        }

        public async Task<Domain.Models.Persistence.Person> UpdatePerson(
            Domain.Models.Persistence.Person command)
        {
            return await Client.AddMessageAndPollAsync<
                Domain.Models.Persistence.Person,
                Domain.Models.Persistence.Person>(
                command,
                QueueConstants.Persistence.Person.UpdateEntity.InputQueue,
                QueueConstants.Persistence.Person.UpdateEntity.OutputQueue
            );
        }

        public async Task<Domain.Models.Persistence.Person> GetPerson(
            string personKey)
        {
            return await Client.AddMessageAndPollAsync<
                string,
                Domain.Models.Persistence.Person>(
                personKey,
                QueueConstants.Persistence.Person.GetEntity.InputQueue,
                QueueConstants.Persistence.Person.GetEntity.OutputQueue
            );
        }

        public async Task<IEnumerable<Domain.Models.Persistence.Person>> GetPeople()
        {
            return await Client.AddRequestAndPollAsync<
                IEnumerable<Domain.Models.Persistence.Person>>(
                QueueConstants.Persistence.Person.GetEntities.InputQueue,
                QueueConstants.Persistence.Person.GetEntities.OutputQueue
            );
        }

        public async Task DeletePerson(string personKey)
        {
            await Client.AddMessageAndPollVoidAsync<string>(
                personKey,
                QueueConstants.Persistence.Person.DeleteEntity.InputQueue,
                QueueConstants.Persistence.Person.DeleteEntity.OutputQueue);
        }
    }

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
