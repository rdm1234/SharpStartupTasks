using SharpStartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.SeparatedStartupTasks
{
    public class SecondSeparetedStartupTask : IStartupTask
    {
        private readonly SomeStartupDependentService _dependentService;

        public SecondSeparetedStartupTask(SomeStartupDependentService dependentService)
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
