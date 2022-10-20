namespace SharpStartupTasks.Tests.SampleStartupTasks
{
    public class SimpleAsynchronousStartupTask : IStartupTask
    {
        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Executed = true;
            return Task.CompletedTask;
        }

        public static bool Executed { get; set; }
    }
}