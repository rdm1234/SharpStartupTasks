using Examples.AspNetCoreWebApi.Services;
using SharpStartupTasks;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.AspNetCoreWebApi.StartupTasks
{
    public class MustRunAfterAllStartupTask : IStartupTask
    {
        private readonly MainStartupDependentService _dependentService;

        public MustRunAfterAllStartupTask(MainStartupDependentService dependentService)
        {
            this._dependentService = dependentService;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var props = typeof(MainStartupDependentService)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.PropertyType == typeof(bool));

            if (props.Select(p => (bool)p.GetValue(_dependentService)).Count(p => p == false) == 2)
            {
                _dependentService.MustRunAfterAllStartupTaskCompletedCorrect = true;
            }

            return Task.CompletedTask;
        }
    }
}
