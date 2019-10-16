using FunctionsTest.Domain.Helpers;
using FunctionsTest.Domain.Models.Constants;
using FunctionsTest.Domain.Models.Persistence;
using FunctionsTest.Domain.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;

namespace PersonService
{
    /*
     * At some point, this is probably going to move to persistence
     * This will also act as the full Persistence Layer implementation for a single entity
     */
    public static class CreatePerson
    {
        /*
         * Can we do any crazy initialization logic up here?  
         * We already have had to grab CloudStorageAccount a couple times...
         * Is there some way we can containerize some of this into our static environment here?
         */

        [FunctionName("Persistence-Person-Create")]
        public static void Run(
            [QueueTrigger(QueueConstants.Persistence.Person.CreateEntity.InputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)]string myQueueItem,
            [Queue(QueueConstants.Persistence.Person.CreateEntity.OutputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log)
        {
            var person = myQueueItem.GetQueueMessage<Person>();

            var personCollection = ClientGenerator.GetMongoCollection<Person>();

            personCollection.InsertOne(person);

            myQueueResult = myQueueItem.ToQueueResponse(person);
        }
    }

    public static class UpdatePerson
    {
        /*
         * Can we do any crazy initialization logic up here?  
         * We already have had to grab CloudStorageAccount a couple times...
         * Is there some way we can containerize some of this into our static environment here?
         */

        [FunctionName("Persistence-Person-Update")]
        public static void Run(
            [QueueTrigger(QueueConstants.Persistence.Person.UpdateEntity.InputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)]string myQueueItem,
            [Queue(QueueConstants.Persistence.Person.UpdateEntity.OutputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log)
        {
            var person = myQueueItem.GetQueueMessage<Person>();

            var personCollection = ClientGenerator.GetMongoCollection<Person>();

            /*
             * Probably a horrible update pattern, but this isn't a mongo demonstration
             */
            personCollection.FindOneAndReplace(x => x.Id == person.Id, person);

            myQueueResult = myQueueItem.ToQueueResponse(person);
        }
    }

    public static class GetPerson
    {
        [FunctionName("Persistence-Person-Get")]
        public static void Run(
            [QueueTrigger(QueueConstants.Persistence.Person.GetEntity.InputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] string myQueueItem,
            [Queue(QueueConstants.Persistence.Person.GetEntity.OutputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log)
        {
            var personKey = myQueueItem.GetQueueMessage<string>();

            var personCollection = ClientGenerator.GetMongoCollection<Person>();

            var result = personCollection.FindSync(x => x.Id == personKey).SingleOrDefault();

            myQueueResult = myQueueItem.ToQueueResponse(result);
        }
    }

    public static class GetPeople
    {
        [FunctionName("Persistence-People-Get")]
        public static void Run(
            [QueueTrigger(QueueConstants.Persistence.Person.GetEntities.InputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] string myQueueItem,
            [Queue(QueueConstants.Persistence.Person.GetEntities.OutputQueue, 
                Connection = ConfigurationConstants.AzureStorageKey)] out string myQueueResult,
            ILogger log)
        {
            var personCollection = ClientGenerator.GetMongoCollection<Person>();

            var result = personCollection.Find(_ => true).ToList();

            myQueueResult = myQueueItem.ToQueueResponse(result);
        }
    }

    public static class DeletePerson
    {
        [FunctionName("Persistence-Person-Delete")]
        public static void Run(
            [QueueTrigger(QueueConstants.Persistence.Person.DeleteEntity.InputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] string myQueueItem,
            [Queue(QueueConstants.Persistence.Person.DeleteEntity.OutputQueue,
                Connection = ConfigurationConstants.AzureStorageKey)] out string outputQueueItem,
            ILogger log)
        {
            var personCollection = ClientGenerator.GetMongoCollection<Person>();

            var personKey = myQueueItem.GetQueueMessage<string>();

            personCollection.DeleteOne(x => x.Id == personKey);

            outputQueueItem = myQueueItem.ToVoidQueueResponse();
        }
    }


    /*
     * This should simplify our code executions above
     */
    public static class ServiceHelper
    {
        public static void MongoFunction<TEntity, TResult>(
            string myQueueItem, ref string myQueueResult,
            Func<IMongoCollection<TEntity>, TResult> collectionFn)
        {
            var collection = ClientGenerator.GetMongoCollection<TEntity>();

            var result = collectionFn(collection);

            myQueueResult = myQueueItem.ToQueueResponse(result);
        }
    }

}
