using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FunctionsTest.Infrastructure.Extensions;
using FunctionsTest.Domain.Models.Application;
using FunctionsTest.Domain.Helpers;
using Newtonsoft.Json;

namespace PersonController
{
    public static class PersonControllerCreate
    {
        [FunctionName("Presentation-Person-Create")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var body = await req.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<CreatePersonCommand>(body);

            /*
             * We need a better integrated way of handling validation,
             * this would need to be bubbled down to application or managed through 
             * like an ISelfValidate interface on all the application commands
             * then we could offload the model validation to the generated object from
             * the request and test it with simple unit tests.
             */
            if (request.Name == null)
            {
                return new BadRequestObjectResult("Please pass a name on the query string");
            }

            var result = await ClientGenerator.
                GenerateQueueClient().
                GetApplicationQueueClient().
                CreatePerson(request);

            return new OkObjectResult(result.Id);
        }
    }

    public static class PersonControllerGet
    { 
        [FunctionName("Presentation-Person-Get")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var query = new GetPersonQuery
            {
                PersonKey = req.Query["id"]
            };

            var result = await ClientGenerator.
                GenerateQueueClient().
                GetApplicationQueueClient().
                GetPerson(query);

            return new OkObjectResult(result);
        }
    }

    public static class PersonControllerGetAll
    {
        [FunctionName("Presentation-Person-GetAll")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
                ILogger log,
                ExecutionContext context)
        {
            var result = await ClientGenerator.
                GenerateQueueClient().
                GetApplicationQueueClient().
                GetPeople();

            return new OkObjectResult(result);
        }
    }

    public static class PersonControllerDelete
    {
        [FunctionName("Presentation-Person-Delete")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
                ILogger log,
                ExecutionContext context)
        {
            var command = JsonConvert.DeserializeObject<DeletePersonCommand>(await req.ReadAsStringAsync());

            await ClientGenerator.
                GenerateQueueClient().
                GetApplicationQueueClient().
                DeletePerson(command);

            return new EmptyResult();
        }
    }

    public static class PersonControllerUpdate
    {
        [FunctionName("Presentation-Person-Update")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
                ILogger log,
                ExecutionContext context)
        {
            var model = JsonConvert.DeserializeObject<UpdatePersonCommand>(await req.ReadAsStringAsync());

            var result = await ClientGenerator.
                GenerateQueueClient().
                GetApplicationQueueClient().
                UpdatePerson(model);

            return new OkObjectResult(result);
        }
    }
}


