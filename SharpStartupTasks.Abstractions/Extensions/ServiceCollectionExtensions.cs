using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpStartupTasks.Abstractions;
using SharpStartupTasks.Abstractions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpStartupTasks.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions for adding startup tasks to the DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static void AddStartupTasksConfiguration(this IServiceCollection services, StartupConfigurationNode configuration, Type taskType)
        {
            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration
            {
                CurrentSet = configuration,
                Tasks = new() { new() { Type = taskType } }
            });
        }
        
        private static void AddStartupTasksConfiguration(this IServiceCollection services, StartupConfigurationNode configuration, List<Type> taskTypes)
        {
            var tasks = taskTypes.Select(t => new StartupTaskConfiguration
            {
                Type = t
            }).ToList();

            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration
            {
                CurrentSet = configuration,
                Tasks = tasks
            });
        }
        
        private static string AddAnonymousTaskConfiguration(this IServiceCollection services, StartupConfigurationNode configuratioNode)
        {
            var id = Guid.NewGuid().ToString();

            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                CurrentSet = configuratioNode,
                Tasks = new() { new() { AnonymousTaskId = id } }
            });

            return id;
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <typeparamref name="T"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf<T>(this IServiceCollection services)
        {
            return services.AddStartupTasksFromAssembleyOf(typeof(T));
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <typeparamref name="T"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf<T>(this IServiceCollection services, StartupConfigurationNode configuration)
        {
            return services.AddStartupTasksFromAssembleyOf(typeof(T), configuration);
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <paramref name="ofType"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf(this IServiceCollection services, Type ofType)
        {
            ActualAddStartupTasksFromAssembleyOf(services, ofType);
            return services;
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <paramref name="ofType"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf(this IServiceCollection services, Type ofType, StartupConfigurationNode configuration)
        {
            var startupTaskTypes = ActualAddStartupTasksFromAssembleyOf(services, ofType);
            services.AddStartupTasksConfiguration(configuration, startupTaskTypes);
            return services;
        }

        private static List<Type> ActualAddStartupTasksFromAssembleyOf(IServiceCollection services, Type ofType)
        {
            var startupTaskTypes = ofType.Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IBaseStartupTask)))
                .ToList();

            foreach (var taskType in startupTaskTypes)
            {
                if (taskType.IsAssignableTo(typeof(IStartupTask)))
                {
                    services.AddStartupTask(taskType);
                }
                else
                {
                    services.AddSyncStartupTask(taskType);
                }
            }

            return startupTaskTypes;
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <typeparamref name="T"/> except <paramref name="exceptTypes"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf<T>(this IServiceCollection services, params Type[] exceptTypes)
        {
            return services.AddStartupTasksFromAssembleyOf(typeof(T), exceptTypes);
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <paramref name="ofType"/> except <paramref name="exceptTypes"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf(this IServiceCollection services, Type ofType, params Type[] exceptTypes)
        {
            var startupTaskTypes = ofType.Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IBaseStartupTask)))
                .Except(exceptTypes);

            foreach (var taskType in startupTaskTypes)
            {
                if (taskType.IsAssignableTo(typeof(IStartupTask)))
                {
                    services.AddStartupTask(taskType);
                }
                else
                {
                    services.AddSyncStartupTask(taskType);
                }
            }

            return services;
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <typeparamref name="T"/> except <paramref name="orderedStartupTasks"/> then add all <paramref name="orderedStartupTasks"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyThenOrdered<T>(
            this IServiceCollection services, 
            params Type[] orderedStartupTasks)
        {
            return services.AddStartupTasksFromAssembleyThenOrdered(typeof(T), orderedStartupTasks);
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <typeparamref name="T"/> except <paramref name="fromAssembleyOf"/> then add all <paramref name="orderedStartupTasks"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyThenOrdered(
            this IServiceCollection services,
            Type fromAssembleyOf,
            params Type[] orderedStartupTasks)
        {
            services.AddStartupTasksFromAssembleyOf(fromAssembleyOf, orderedStartupTasks);

            foreach (var t in orderedStartupTasks)
            {
                services.AddStartupTask(t);
            }

            return services;
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <typeparamref name="T"/> except <paramref name="orderedStartupTasks"/> and <paramref name="exceptTypes"/> then add all <paramref name="orderedStartupTasks"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyThenOrdered<T>(
            this IServiceCollection services, 
            Type[] orderedStartupTasks,
            Type[] exceptTypes)
        {
            return services.AddStartupTasksFromAssembleyThenOrdered(typeof(T), orderedStartupTasks, exceptTypes);
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <paramref name="fromAssembleyOf"/> except <paramref name="orderedStartupTasks"/> and <paramref name="exceptTypes"/> then add all <paramref name="orderedStartupTasks"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyThenOrdered(
            this IServiceCollection services,
            Type fromAssembleyOf,
            Type[] orderedStartupTasks,
            Type[] exceptTypes)
        {
            return services.AddStartupTasksFromAssembleyThenOrdered(fromAssembleyOf, orderedStartupTasks.AsEnumerable(), exceptTypes.AsEnumerable());
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <paramref name="fromAssembleyOf"/> except <paramref name="orderedStartupTasks"/> and <paramref name="exceptTypes"/> then add all <paramref name="orderedStartupTasks"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyThenOrdered(
            this IServiceCollection services,
            Type fromAssembleyOf,
            IEnumerable<Type> orderedStartupTasks,
            IEnumerable<Type> exceptTypes)
        {
            services.AddStartupTasksFromAssembleyOf(fromAssembleyOf, orderedStartupTasks.Union(exceptTypes));

            foreach (var t in orderedStartupTasks)
            {
                services.AddStartupTask(t);
            }

            return services;
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <paramref name="ofType"/> except <paramref name="exceptTypes"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf(this IServiceCollection services, Type ofType, IEnumerable<Type> exceptTypes)
        {
            var startupTaskTypes = ofType.Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IBaseStartupTask)))
                .Except(exceptTypes);

            foreach (var taskType in startupTaskTypes)
            {
                if (taskType.IsAssignableTo(typeof(IStartupTask)))
                {
                    services.AddStartupTask(taskType);
                }
                else
                {
                    services.AddSyncStartupTask(taskType);
                }
            }

            return services;
        }

        /// <summary>
        /// Add startup tasks to <paramref name="services"/> as scoped (can add both <see cref="IStartupTask"/> and <see cref="ISyncStartupTask"/> at the same time)
        /// </summary>
        public static IServiceCollection AddMixedStartupTasks(this IServiceCollection services, params Type[] startupTasks)
        {
            foreach (var st in startupTasks)
            {
                services.AddScoped(typeof(IBaseStartupTask), st);
            }

            return services;
        }

        #region Simple add tasks
        // TODO: Decide which to use: IBaseStartupTask or concrete types
        /// <summary>
        /// Add startup task of type <typeparamref name="TTask"/>
        /// </summary>
        public static IServiceCollection AddStartupTask<TTask>(this IServiceCollection services)
            where TTask : class, IStartupTask
        {
            return services.AddStartupTask(typeof(TTask));
        }

        /// <summary>
        /// Add startup task of type <typeparamref name="TTask"/>
        /// </summary>
        public static IServiceCollection AddStartupTask<TTask>(this IServiceCollection services, StartupConfigurationNode configuration)
            where TTask : class, IStartupTask
        {
            return services.AddStartupTask(typeof(TTask), configuration);
        }

        /// <summary>
        /// Add startup task of type <paramref name="taskType"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Type taskType)
        {
            return services.AddScoped(typeof(IBaseStartupTask), taskType);
        }

        /// <summary>
        /// Add startup task of type <paramref name="taskType"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Type taskType, StartupConfigurationNode configuration)
        {
            AddStartupTasksConfiguration(services, configuration, taskType);
            return services.AddScoped(typeof(IBaseStartupTask), taskType);
        }

        /// <summary>
        /// Add sync startup task
        /// </summary>
        public static IServiceCollection AddSyncStartupTask<TSyncTask>(this IServiceCollection services)
            where TSyncTask : class, ISyncStartupTask
        {
            return services.AddSyncStartupTask(typeof(TSyncTask));
        }

        /// <summary>
        /// Add sync startup task
        /// </summary>
        public static IServiceCollection AddSyncStartupTask<TSyncTask>(this IServiceCollection services, StartupConfigurationNode configuration)
            where TSyncTask : class, ISyncStartupTask
        {
            return services.AddSyncStartupTask(typeof(TSyncTask), configuration);
        }

        /// <summary>
        /// Add startup task of type <paramref name="taskType"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Type taskType)
        {
            return services.AddScoped(typeof(IBaseStartupTask), taskType);
        }

        /// <summary>
        /// Add startup task of type <paramref name="taskType"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Type taskType, StartupConfigurationNode configuration)
        {
            AddStartupTasksConfiguration(services, configuration, taskType);
            return services.AddScoped(typeof(IBaseStartupTask), taskType);
        }

        #endregion

        #region Add anonymous startup tasks

        /// <summary>
        /// Add <see cref="AnonymousAsyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<IServiceProvider, CancellationToken, Task> task, string? id = null)
        {
            return services.AddScoped<IBaseStartupTask>(sp => new AnonymousAsyncStartupTask(task, sp, id));
        }

        /// <summary>
        /// Add <see cref="AnonymousAsyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<CancellationToken, Task> task, string? id = null)
        {
            return services.AddStartupTask((sp, ct) => task(ct), id);
        }

        /// <summary>
        /// Add <see cref="AnonymousAsyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<Task> task, string? id = null)
        {
            return services.AddStartupTask(ct => task(), id);
        }

        /// <summary>
        /// Add <see cref="AnonymousAsyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<IServiceProvider, CancellationToken, Task> task, StartupConfigurationNode configuratioNode)
        {
            var id = AddAnonymousTaskConfiguration(services, configuratioNode);
            return services.AddStartupTask(task, id);
        }

        /// <summary>
        /// Add <see cref="AnonymousAsyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<CancellationToken, Task> task, StartupConfigurationNode configuratioNode)
        {
            var id = AddAnonymousTaskConfiguration(services, configuratioNode);
            return services.AddStartupTask(task, id);
        }

        /// <summary>
        /// Add <see cref="AnonymousAsyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<Task> task, StartupConfigurationNode configuratioNode)
        {
            var id = AddAnonymousTaskConfiguration(services, configuratioNode);
            return services.AddStartupTask(task, id);
        }

        /// <summary>
        /// Add <see cref="AnonymousSyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Action<IServiceProvider, CancellationToken> task, string? id = null)
        {
            return services.AddScoped<IBaseStartupTask>(sp => new AnonymousSyncStartupTask(task, sp, id));
        }

        /// <summary>
        /// Add <see cref="AnonymousSyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Action<CancellationToken> task, string? id = null)
        {
            return services.AddSyncStartupTask((sp, ct) => task(ct), id);
        }
        
        /// <summary>
        /// Add <see cref="AnonymousSyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Action task, string? id = null)
        {
            return services.AddSyncStartupTask(ct => task(), id);
        }

        /// <summary>
        /// Add <see cref="AnonymousSyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Action<IServiceProvider, CancellationToken> task, StartupConfigurationNode configuratioNode)
        {
            var id = AddAnonymousTaskConfiguration(services, configuratioNode);
            return services.AddSyncStartupTask(task, id);
        }
        
        /// <summary>
        /// Add <see cref="AnonymousSyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Action<CancellationToken> task, StartupConfigurationNode configuratioNode)
        {
            var id = AddAnonymousTaskConfiguration(services, configuratioNode);
            return services.AddSyncStartupTask(task, id);
        }

        /// <summary>
        /// Add <see cref="AnonymousSyncStartupTask"/> which runs <paramref name="task"/>
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Action task, StartupConfigurationNode configuratioNode)
        {
            var id = AddAnonymousTaskConfiguration(services, configuratioNode);
            return services.AddSyncStartupTask(task, id);
        }

        #endregion

        #region Add tasks from factory methods
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<IServiceProvider, IStartupTask> factory)
        {
            return services.AddScoped<IBaseStartupTask>(factory);
        }
        #endregion

        #region Configuration

        /// <summary>
        /// Adds configuration for startup tasks. Multipe configurations can be provided (e. g. from different projects)
        /// </summary>
        public static IServiceCollection AddStartupTasksConfiguration(this IServiceCollection services, SharpStartupTasksConfiguration configuration)
        {
            services.AddSingleton(configuration);
            return services;
        }

        /// <summary>
        /// Adds configuration for startup tasks. Multipe configurations can be provided (e. g. from different projects)
        /// </summary>
        public static IServiceCollection AddStartupTasksConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.GetSection("SharpStartupTasks").Get<SharpStartupTasksConfiguration>();
            return services.AddStartupTasksConfiguration(cfg);
        }
        
        #endregion
    }
}
