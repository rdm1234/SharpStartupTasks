using SharpStartupTasks;
using System.Threading;

namespace Examples.SeparatedStartupTasks
{
    public class SyncSeparetedStartupTask : ISyncStartupTask
    {
        private readonly SomeStartupDependentService _dependentService;

        public SyncSeparetedStartupTask(SomeStartupDependentService dependentService)
        {
            this._dependentService = dependentService;
        }

        public void Execute(CancellationToken cancellationToken = default)
        {
            _dependentService.SyncStartupTaskCompleted = true;
        }
    }
}
