using Newtonsoft.Json;
using System;

namespace FunctionsTest.AzureQueue.Extensions
{
    /*
     * This piece is reusable for any usage of the Queue Technology
     */
    public static class QueueMessageExtensions
    {
        public const char Delimiter = '|';

        public static string ToQueueMessage(this string message, Guid id)
        {
            return $"{id.ToString()}{Delimiter}{message}";
        }

        public static string ToQueueMessage(this object message, Guid id)
        {
            return JsonConvert.SerializeObject(message).ToQueueMessage(id);
        }

        public static string ToVoidQueueResponse(this string originalMessage)
        {
            var returnId = originalMessage.GetQueueMessageId();
            return returnId + Delimiter + "{}";
        }

        public static string ToQueueResponse(this string originalMessage, string message)
        {
            var returnId = originalMessage.GetQueueMessageId();
            return $"{returnId}{Delimiter}{message}";
        }

        public static string ToQueueResponse<T>(this string originalMessage, T message)
        {
            var returnId = originalMessage.GetQueueMessageId();
            return $"{returnId}{Delimiter}{JsonConvert.SerializeObject(message)}";
        }

        public static string GetQueueMessageId(this string message)
        {
            var parts = GetQueueParts(message);
            return parts[0];
        }

        public static string GetQueueMessage(this string message)
        {
            var parts = GetQueueParts(message);

            if (parts.Length == 2) {
                return parts[1];
            }
            return parts[0];
        }

        public static T GetQueueMessage<T>(this string message)
        {
            var messageString = message.GetQueueMessage();
            return JsonConvert.DeserializeObject<T>(messageString);
        }

        private static string[] GetQueueParts(string message)
        {
            var messageParts = message.Split(Delimiter);

            if (messageParts.Length > 2)
            {
                throw new Exception($"Invalid queue message.  More than 2 parts.  Message: {message}");
            }

            return messageParts;
        }
    }
}
