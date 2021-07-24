using Examples.AspNetCoreWebApi.Services;
using SharpStartupTasks;
using System.Threading;

namespace Examples.AspNetCoreWebApi.StartupTasks
{
    public class SyncFirstStartupTask : ISyncStartupTask
    {
        private readonly MainStartupDependentService _dependentService;

        public SyncFirstStartupTask(MainStartupDependentService dependentService)
        {
            this._dependentService = dependentService;
        }

        public void Execute(CancellationToken cancellationToken = default)
        {
            _dependentService.FirstSyncStartupTaskCompleted = true;
        }
    }
}