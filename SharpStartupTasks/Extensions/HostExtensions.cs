using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SharpStartupTasks.Extensions
{
    /// <summary>
    /// <see cref="IHost"/> extensions for running startup tasks on app start
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Execute all startup tasks (sync first) using <see cref="RunStartupTasksAsync(IHost, ILogger, CancellationToken)"/> and execute RunAsync(<paramref name="cancellationToken"/>) on <paramref name="host"/>
        /// </summary>
        public static async Task RunWithTasksAsync(this IHost host, ILogger logger = null, CancellationToken cancellationToken = default)
        {
            logger?.LogDebug("Running startup tasks");

            await RunStartupTasksAsync(host, logger, cancellationToken);

            logger?.LogDebug($"Executing RunAsync on host");

            await host.RunAsync(cancellationToken);
        }

        /// <summary>
        /// Execute all startup tasks strictly in the order they were added
        /// </summary>
        public static async Task RunStartupTasksAsync(IHost host, ILogger logger = null, CancellationToken cancellationToken = default)
        {
            var scope = host.Services.CreateScope();

            var startupTasks = scope.ServiceProvider.GetServices<IBaseStartupTask>();

            foreach (var st in startupTasks)
            {
                if (st is IStartupTask asyncStartupTask)
                {
                    logger?.LogInformation("Executing async startup task of type '{TypeName}'", st.GetType().FullName);

                    await asyncStartupTask.ExecuteAsync(cancellationToken);
                }
                else
                {
                    logger?.LogInformation("Executing sync startup task of type '{TypeName}'", st.GetType().FullName);

                    ((ISyncStartupTask)st).Execute(cancellationToken);
                }
            }
        }
    }
}
