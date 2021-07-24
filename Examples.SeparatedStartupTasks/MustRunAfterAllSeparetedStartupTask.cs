using SharpStartupTasks;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.SeparatedStartupTasks
{
    public class MustRunAfterAllSeparetedStartupTask : IStartupTask
    {
        private readonly SomeStartupDependentService _dependentService;

        public MustRunAfterAllSeparetedStartupTask(SomeStartupDependentService dependentService)
        {
            this._dependentService = dependentService;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var props = typeof(SomeStartupDependentService)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.PropertyType == typeof(bool));

            if (props.Select(p => (bool)p.GetValue(_dependentService)).Count(p => p == false) == 1)
            {
                _dependentService.MustRunAfterAllTaskCompledCorrect = true;
            }


            return Task.CompletedTask;
        }
    }
}
