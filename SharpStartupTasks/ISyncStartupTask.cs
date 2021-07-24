using System.Threading;

namespace SharpStartupTasks
{
    public interface ISyncStartupTask : IBaseStartupTask
    {
        public void Execute(CancellationToken cancellationToken = default);
    }
}
