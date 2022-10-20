namespace SharpStartupTasks.Tests.SampleStartupTasks;

public class SimpleSynchronousStartupTask : ISyncStartupTask
{
    public void Execute(CancellationToken cancellationToken = default)
    {
        Executed = true;
    }

    public static bool Executed { get; private set; }
}
