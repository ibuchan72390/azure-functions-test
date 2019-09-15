using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
using System.Linq;
using FunctionsTest.Domain.Extensions;
using FunctionsTest.Domain.Models.Constants;

namespace HttpFunction
{
    public static class HttpFunction
    {
        [FunctionName("HttpFunction")]
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
                // This should happen somewhere else...perhaps an initialization utility of some form?
                // Perhaps that utility could provide the CloudStorageClient in return as well
                CloudQueue inputQueue = queueClient.GetQueueReference(
                    QueueConstants.Application.SaveQueueMessage.InputQueue
                );
                await inputQueue.CreateIfNotExistsAsync();

                CloudQueue outputQueue = queueClient.GetQueueReference(
                    QueueConstants.Application.SaveQueueMessage.OutputQueue    
                );
                await outputQueue.CreateIfNotExistsAsync();



                /*
                 * This entire piece can be replicated...
                 * Useful variables to extract...
                 *  - Message Count
                 */
                #region Polling Process (Could we somehow use the Polly process here?)

                CloudQueueMessage inputMessage = new CloudQueueMessage(name.ToQueueMessage(context.InvocationId), false);
                await inputQueue.AddMessageAsync(inputMessage);

                string stringId = context.InvocationId.ToString();
                CloudQueueMessage result = null;

                while (result == null || result.AsString.GetQueueMessageId() != stringId)
                {
                    /*
                     * This is not sufficient simply because if we get one message stuck at the top of the queue without a recipient, the entire chain fails.
                     * We need a way of moving beyond the top item on the queue and checking the other results.
                     */
                    var msgs = await outputQueue.PeekMessagesAsync(10);
                    var msgDictionary = msgs.ToDictionary(x => x.AsString.GetQueueMessageId(), x => x);

                    if (msgDictionary.ContainsKey(stringId))
                    {
                        var results = await outputQueue.GetMessagesAsync(10);
                        result = results.ToDictionary(x => x.AsString.GetQueueMessageId(), x => x)[stringId];
                    }
                }

                await outputQueue.DeleteMessageAsync(result);

                #endregion

                return name != null
                    ? (ActionResult)new OkObjectResult(result.AsString)
                    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                throw;
            }
        }

        //private static string GetCloudQueueMessageId(CloudQueueMessage msg)
        //{
        //    return msg.AsString.Split(delimiter)[0];
        //}
    }
}


