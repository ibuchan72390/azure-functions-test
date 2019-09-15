namespace FunctionsTest.Domain.Models.Constants
{
    public static class QueueConstants
    {
        public static class Application
        {
            public static class SaveQueueMessage
            {
                // Need much better names for this bullshit
                public static string InputQueue = "test-input";
                public static string OutputQueue = "test-output";
            }

        }

        public static class Persistence
        { }
    }
}
