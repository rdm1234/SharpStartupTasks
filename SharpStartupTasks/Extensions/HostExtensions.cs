using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpStartupTasks.Abstractions.Configuration;
using SharpStartupTasks.Services;
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
        /// Execute all startup tasks (sync first) using <see cref="RunStartupTasksAsync(IHost, CancellationToken)"/> and execute RunAsync(<paramref name="cancellationToken"/>) on <paramref name="host"/>
        /// </summary>
        /// <param name="host"></param>
        /// <param name="logger">
        /// Logger used to show message "Executing RunAsync on host"
        /// </param>
        /// <param name="cancellationToken"></param>
        public static async Task RunWithTasksAsync(this IHost host, ILogger? logger = null, CancellationToken cancellationToken = default)
        {
            await RunStartupTasksAsync(host, cancellationToken);

            logger?.LogDebug($"Executing RunAsync on host");

            await host.RunAsync(cancellationToken);
        }

        /// <summary>
        /// Execute all startup tasks strictly in the order they were added
        /// </summary>
        public static async Task RunStartupTasksAsync(IHost host, CancellationToken cancellationToken = default)
        {
            var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var startupTasksRunner = serviceProvider.GetRequiredService<IStartupTasksRunner>();

            await startupTasksRunner.RunTasksAsync(cancellationToken);
        }
    }
}
