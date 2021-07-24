using Examples.AspNetCoreWebApi.Services;
using SharpStartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.AspNetCoreWebApi.StartupTasks
{
    public class FirstStartupTask : IStartupTask
    {
        private readonly MainStartupDependentService _dependentService;

        public FirstStartupTask(MainStartupDependentService dependentService)
        {
            this._dependentService = dependentService;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _dependentService.FirstStartupTaskCompleted = true;

            return Task.CompletedTask;
        }
    }
}