namespace ExampleLongPollingWithTaskCompletionSource.Api
{
    public class TaskTimeoutOptions
    {
        public const string ConfigSectionName = "Task";

        public int TimeoutIntSecounds { get; set; }
    }
}
