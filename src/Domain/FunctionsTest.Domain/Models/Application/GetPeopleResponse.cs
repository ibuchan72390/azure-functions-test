using FunctionsTest.Domain.Models.Persistence;
using System.Collections.Generic;

namespace FunctionsTest.Domain.Models.Application
{
    public class GetPeopleResponse
    {
        public GetPeopleResponse()
        {
            People = new List<Person>();
        }

        public IEnumerable<Person> People { get; set; }
    }
}