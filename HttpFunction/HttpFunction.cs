using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;

namespace HttpFunction
{
    public static class HttpFunction
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            try
            {
                string webJobStorage = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

                // Setup our StorageAccount
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(webJobStorage);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to the required containers
                CloudQueue inputQueue = queueClient.GetQueueReference("test-input");
                await inputQueue.CreateIfNotExistsAsync();

                CloudQueue outputQueue = queueClient.GetQueueReference("test-output");
                await outputQueue.CreateIfNotExistsAsync();

                CloudQueueMessage inputMessage = new CloudQueueMessage(name, false);
                await inputQueue.AddMessageAsync(inputMessage);

                return name != null
                    ? (ActionResult)new OkObjectResult(webJobStorage)
                    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                throw;
            }

        }
    }
}

public class Options {
    public bool IsEncrypted { get; set; }
    public OptionsValues Values { get; set; }
}

public class OptionsValues {
    public string AzureWebJobsStorage { get; set; }
    public string FUNCTIONS_WORKER_RUNTIME { get; set; }
}