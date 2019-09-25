using FunctionsTest.Domain.Models.Constants;
using Microsoft.Azure.Storage.Queue;
using MongoDB.Driver;
using System;

namespace FunctionsTest.Domain.Helpers
{
    public static class ClientGenerator
    {
        public static CloudQueueClient GenerateQueueClient()
        {
            string webJobStorage = Environment.GetEnvironmentVariable(
                ConfigurationConstants.AzureStorageKey);

            Microsoft.Azure.Storage.CloudStorageAccount storageAccount = 
                Microsoft.Azure.Storage.CloudStorageAccount.Parse(webJobStorage);

            return storageAccount.CreateCloudQueueClient();
        }

        public static IMongoClient GetMongoClient()
        {
            string mongoConnString = Environment.GetEnvironmentVariable(
                ConfigurationConstants.MongoConnectionString);

            return new MongoClient(mongoConnString);
        }

        public static IMongoDatabase GetMongoDatabase()
        {
            string mongoDbString = Environment.GetEnvironmentVariable(
                ConfigurationConstants.MongoDatabaseName);

            return GetMongoClient().GetDatabase(mongoDbString);
        }

        public static IMongoCollection<T> GetMongoCollection<T>()
        {
            return GetMongoDatabase().GetCollection<T>(typeof(T).Name);
        }
    }
}
