using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FunctionsTest.Domain.Extensions;
using FunctionsTest.Domain.Models.Constants;
using FunctionsTest.Domain.Models.Persistence;
using FunctionsTest.Domain.Helpers;
using FunctionsTest.Domain.Models.Application;

namespace FunctionsTest.Application
{
    public static class CreatePersonHandler
    {
        /*
         * There's got to be some way we can wrap this into some form of inherent Typing
         * Once we get a proper type system, we can automatically convert these for the context
         * 
         * Need to know the input queue and it's type as well as the output queue and its type
         * The rest of that can be handled in a wrapper function
         */
        [FunctionName("Application-Person-Create")]
        public static void Run(
            [QueueTrigger(QueueConstants.Application.Person.CreateEntity.InputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] string myQueueItem, 
            [Queue(QueueConstants.Application.Person.CreateEntity.OutputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log,
            ExecutionContext context)
        {
            var command = myQueueItem.GetQueueMessage<CreatePersonCommand>();

            var entity = new Person { Name = command.Name };

            var result = ClientGenerator.
                GenerateQueueClient().
                GetPersistenceQueueClient().
                CreatePerson(entity).
                Result;

            myQueueResult = myQueueItem.ToQueueResponse(result);
        }
    }

    public static class GetPersonHandler
    {
        [FunctionName("Application-Person-Get")]
        public static void Run(
            [QueueTrigger(QueueConstants.Application.Person.GetEntity.InputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey) ]string myQueueItem,
            [Queue(QueueConstants.Application.Person.GetEntity.OutputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log,
            ExecutionContext context)
        {
            var query = myQueueItem.GetQueueMessage<GetPersonQuery>();

            var result = ClientGenerator.
                GenerateQueueClient().
                GetPersistenceQueueClient().
                GetPerson(query.PersonKey).
                Result;

            var response = new GetPersonResponse
            {
                Person = result
            };

            myQueueResult = myQueueItem.ToQueueResponse(response);
        }
    }


    public static class GetPeopleHandler
    {
        [FunctionName("Application-People-Get")]
        public static void Run(
            [QueueTrigger(QueueConstants.Application.Person.GetEntities.InputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] string myQueueItem,
            [Queue(QueueConstants.Application.Person.GetEntities.OutputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log,
            ExecutionContext context)
        {
            var result = ClientGenerator.
                GenerateQueueClient().
                GetPersistenceQueueClient().
                GetPeople().
                Result;

            var response = new GetPeopleResponse
            {
                People = result
            };

            myQueueResult = myQueueItem.ToQueueResponse(response);
        }
    }

    public static class UpdatePersonHandler
    {
        [FunctionName("Application-Person-Update")]
        public static void Run(
            [QueueTrigger(QueueConstants.Application.Person.UpdateEntity.InputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] string myQueueItem,
            [Queue(QueueConstants.Application.Person.UpdateEntity.OutputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log,
            ExecutionContext context)
        {
            var command = myQueueItem.GetQueueMessage<UpdatePersonCommand>();

            var persistenceClient = ClientGenerator.
                GenerateQueueClient().
                GetPersistenceQueueClient();

            var person = persistenceClient.
                GetPerson(command.Id).
                Result;

            person.Name = command.Name;

            person = persistenceClient.UpdatePerson(person).Result;

            var response = new UpdatePersonResponse { Person = person };

            myQueueResult = myQueueItem.ToQueueResponse(response);
        }
    }

    public static class DeletePersonHandler
    {
        [FunctionName("Application-Person-Delete")]
        public static void Run(
            [QueueTrigger(QueueConstants.Application.Person.DeleteEntity.InputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] string myQueueItem,
            [Queue(QueueConstants.Application.Person.DeleteEntity.InputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log,
            ExecutionContext context)
        {
            var command = myQueueItem.GetQueueMessage<DeletePersonCommand>();

            ClientGenerator.
                GenerateQueueClient().
                GetPersistenceQueueClient().
                DeletePerson(command.PersonKey).
                ConfigureAwait(true).
                GetAwaiter().
                GetResult();

            myQueueResult = myQueueItem.ToQueueResponse(new DeletePersonResponse());
        }
    }
}
