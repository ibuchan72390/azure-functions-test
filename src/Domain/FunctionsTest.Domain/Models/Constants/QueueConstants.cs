namespace FunctionsTest.Domain.Models.Constants
{
    public static class QueueConstants
    {
        // Shared Keys
        private const string Base = "functionstest-";
        private const string Input = "input";
        private const string Output = "output";

        private const string Read = "read";
        private const string ReadAll = "readall";
        private const string Delete = "delete";
        private const string Create = "create";
        private const string Update = "update";


        public static class Application
        {
            private const string App = Base + "application-";

            public static class Person
            {
                private const string PersonKey = App + "person-";

                public static class CreateEntity
                {
                    public const string InputQueue = PersonKey + Create + Input;
                    public const string OutputQueue = PersonKey + Create + Output;
                }

                public static class UpdateEntity
                {
                    public const string InputQueue = PersonKey + Update + Input;
                    public const string OutputQueue = PersonKey + Update + Output;
                }

                public static class DeleteEntity
                {
                    public const string InputQueue = PersonKey + Delete + Input;
                    public const string OutputQueue = PersonKey + Delete + Output;
                }

                public static class GetEntity
                {
                    public const string InputQueue = PersonKey + Read + Input;
                    public const string OutputQueue = PersonKey + Read + Output;
                }

                public static class GetEntities
                {
                    public const string InputQueue = PersonKey + ReadAll + Input;
                    public const string OutputQueue = PersonKey + ReadAll + Output;
                }
            }
        }

        public static class Persistence
        {
            private const string Pers = Base + "persistence-";

            public static class Person
            {
                private const string PersonKey = Pers + "person-";

                public static class CreateEntity
                {
                    public const string InputQueue = PersonKey + Create + Input;
                    public const string OutputQueue = PersonKey + Create + Output;
                }

                public static class UpdateEntity
                {
                    public const string InputQueue = PersonKey + Update + Input;
                    public const string OutputQueue = PersonKey + Update + Output;
                }

                public static class DeleteEntity
                {
                    public const string InputQueue = PersonKey + Delete + Input;
                    public const string OutputQueue = PersonKey + Delete + Output;
                }

                public static class GetEntity
                {
                    public const string InputQueue = PersonKey + Read + Input;
                    public const string OutputQueue = PersonKey + Read + Output;
                }

                public static class GetEntities
                {
                    public const string InputQueue = PersonKey + ReadAll + Input;
                    public const string OutputQueue = PersonKey + ReadAll + Output;
                }
            }
        }
    }
}
