using Examples.AspNetCoreWebApi.Services;
using SharpStartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.AspNetCoreWebApi.StartupTasks
{
    public class SecondStartupTask : IStartupTask
    {
        private readonly MainStartupDependentService _dependentService;

        public SecondStartupTask(MainStartupDependentService dependentService)
        {
            this._dependentService = dependentService;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _dependentService.SecondStartupTaskCompleted = true;

            return Task.CompletedTask;
        }
    }
}