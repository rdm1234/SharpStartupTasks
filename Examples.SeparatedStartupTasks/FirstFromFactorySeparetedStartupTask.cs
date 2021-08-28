using SharpStartupTasks;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.SeparatedStartupTasks
{
    public class FirstFromFactorySeparetedStartupTask : IStartupTask
    {
        private readonly SomeStartupDependentService _dependentService;
        private readonly bool _someConfig;

        public FirstFromFactorySeparetedStartupTask(SomeStartupDependentService dependentService, bool someConfig)
        {
            _dependentService = dependentService;
            _someConfig = someConfig;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _dependentService.FirstStartupTaskFromFactory = _someConfig;
            return Task.CompletedTask;
        }
    }
}