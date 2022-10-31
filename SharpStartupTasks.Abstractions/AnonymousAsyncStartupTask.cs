using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpStartupTasks.Abstractions
{
    public class AnonymousAsyncStartupTask : AnonymousStartupTaskBase, IStartupTask
    {
        private readonly Func<IServiceProvider, CancellationToken, Task> _action;
        private readonly IServiceProvider _serviceProvider;

        public AnonymousAsyncStartupTask(Func<IServiceProvider, CancellationToken, Task> action, IServiceProvider serviceProvider, string? id = null) : base(id)
        {
            _action = action;
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _action(_serviceProvider, cancellationToken);
        }
    }
}