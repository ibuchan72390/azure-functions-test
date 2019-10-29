using FunctionsTest.Domain.Models.Constants;
using Microsoft.Azure.Storage.Queue;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionsTest.AzureQueue.Models;
using FunctionsTest.AzureQueue.Extensions;

namespace FunctionsTest.Domain.Extensions
{
    public static class QueueClientExtensions
    {
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
}
