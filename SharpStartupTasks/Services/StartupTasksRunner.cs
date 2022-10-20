using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpStartupTasks.Abstractions.Attributes;
using SharpStartupTasks.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpStartupTasks.Services
{
    public class StartupTasksRunner : IStartupTasksRunner
    {
        private readonly ILogger<StartupTasksRunner>? _logger;
        private readonly IEnumerable<IBaseStartupTask> _startupTasks;
        private readonly IStartupTaskConfigurationProvider _configurationProvider;

        public StartupTasksRunner(
            IServiceProvider serviceProvider,
            IEnumerable<IBaseStartupTask> startupTasks,
            IStartupTaskConfigurationProvider configurationProvider)
        {
            _logger = serviceProvider.GetService<ILogger<StartupTasksRunner>>();
            _startupTasks = startupTasks;
            _configurationProvider = configurationProvider;
        }

        public async Task RunTasksAsync(CancellationToken cancellationToken)
        {
            _logger?.LogDebug("Running startup tasks");


            var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            foreach (var st in _startupTasks)
            {
                var cfg = _configurationProvider.Get(st);

                if (cfg.Disabled!.Value)
                    continue;

                if (cfg.EnabledEnvironments != null && !cfg.EnabledEnvironments.Contains(currentEnv, StringComparer.OrdinalIgnoreCase))
                    continue;

                var ignoreExceptionPredicate = GetIgnoreExceptionPredicate(cfg.IgnoreAllExceptions!.Value, cfg.IgnoreExceptions, cfg.IgnoreExceptionTypes);

                try
                {
                    await RunStartupTaskAsync(st, cancellationToken);
                }
                catch (Exception e)
                {
                    if (!ignoreExceptionPredicate(e))
                    {
                        _logger?.LogCritical(e, "Exception during execution startup task of type '{TypeName}'", st.GetType());
                        throw;
                    }

                    _logger?.LogError(e, "Exception during execution startup task of type '{TypeName}'. Ignoring it.", st.GetType());
                }
            }
        }

        private async Task RunStartupTaskAsync(IBaseStartupTask startupTask, CancellationToken cancellationToken)
        {
            if (startupTask is IStartupTask asyncStartupTask)
            {
                _logger?.LogInformation("Executing async startup task of type '{TypeName}'", startupTask.GetType().FullName);

                await asyncStartupTask.ExecuteAsync(cancellationToken);
            }
            else
            {
                _logger?.LogInformation("Executing sync startup task of type '{TypeName}'", startupTask.GetType().FullName);

                ((ISyncStartupTask)startupTask).Execute(cancellationToken);
            }
        }

        private static Func<Exception, bool> GetIgnoreExceptionPredicate(bool ignoreAllExceptions, List<string>? ignoreExceptionList, Type[]? ignoreExceptionTypeList)
        {
            Func<Exception, bool> ignoreExceptionPredicate;

            if (ignoreAllExceptions)
            {
                ignoreExceptionPredicate = _ => true;
            }
            else if (ignoreExceptionList?.Count > 0)
            {
                ignoreExceptionPredicate = ex =>
                {
                    var exType = ex.GetType();
                    return ignoreExceptionList.Any(exceptionName => exType.Name == exceptionName || exType.FullName == exceptionName);
                };
            }
            else if (ignoreExceptionTypeList?.Length > 0)
            {
                ignoreExceptionPredicate = ex =>
                {
                    var exType = ex.GetType();
                    return ignoreExceptionTypeList.Contains(exType);
                };
            }
            else
            {
                ignoreExceptionPredicate = _ => false;
            }

            return ignoreExceptionPredicate;
        }


    }
}