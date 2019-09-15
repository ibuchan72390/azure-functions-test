using System;

namespace FunctionsTest.Domain.Extensions
{
    public static class QueueMessageStringExtensions
    {
        public const char Delimiter = '|';

        public static string ToQueueMessage(this string message, Guid id)
        {
            return $"{id.ToString()}{Delimiter}{message}";
        }

        public static string GetQueueMessageId(this string message)
        {
            var parts = GetQueueParts(message);
            return parts[0];
        }

        public static string GetQueueMessage(this string message)
        {
            var parts = GetQueueParts(message);
            return parts[1];
        }

        private static string[] GetQueueParts(string message)
        {
            var messageParts = message.Split(Delimiter);

            if (messageParts.Length != 2)
            {
                throw new Exception($"Invalid queue message.  More than 2 parts.  Message: {message}");
            }

            return messageParts;
        }
    }
}
