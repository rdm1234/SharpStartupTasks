using SharpStartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.SeparatedStartupTasks
{
    public class FirstSeparetedStartupTask : IStartupTask
    {
        private readonly SomeStartupDependentService _dependentService;

        public FirstSeparetedStartupTask(SomeStartupDependentService dependentService)
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
