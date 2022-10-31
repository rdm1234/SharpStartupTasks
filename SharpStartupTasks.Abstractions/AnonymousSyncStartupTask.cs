using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpStartupTasks.Abstractions
{
    public class AnonymousSyncStartupTask : AnonymousStartupTaskBase, ISyncStartupTask
    {
        private readonly Action<IServiceProvider, CancellationToken> _action;
        private readonly IServiceProvider _serviceProvider;

        public AnonymousSyncStartupTask(Action<IServiceProvider, CancellationToken> action, IServiceProvider serviceProvider, string? id = null) : base(id)
        {
            _action = action;
            _serviceProvider = serviceProvider;
        }

        public void Execute(CancellationToken cancellationToken = default)
        {
            _action(_serviceProvider, cancellationToken);
        }
    }
}