using SharpStartupTasks;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.SeparatedStartupTasks
{
    public class ShouldNotRunForNowSeparetedStartupTask : IStartupTask
    {
        private readonly SomeStartupDependentService _dependentService;

        public ShouldNotRunForNowSeparetedStartupTask(SomeStartupDependentService dependentService)
        {
            this._dependentService = dependentService;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var props = typeof(SomeStartupDependentService)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.PropertyType == typeof(bool));

            foreach (var p in props)
            {
                p.SetValue(_dependentService, false);
            }

            return Task.CompletedTask;
        }
    }
}
