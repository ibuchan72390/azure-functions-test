using FunctionsTest.Domain.Interfaces.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FunctionsTest.Domain.Models.Persistence
{
    [Serializable]
    public class Person : IPersonData
    {
        #region Attributes

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        #endregion
    }
}
