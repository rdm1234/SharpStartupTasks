using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpStartupTasks.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions for adding startup tasks to the DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <typeparamref name="T"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf<T>(this IServiceCollection services)
        {
            return services.AddStartupTasksFromAssembleyOf(typeof(T));
        }

        /// <summary>
        /// Add all <see cref="IStartupTask"/> from assembley of <paramref name="ofType"/>
        /// </summary>
        public static IServiceCollection AddStartupTasksFromAssembleyOf(this IServiceCollection services, Type ofType)
        {
            var startupTaskTypes = ofType.Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IBaseStartupTask)));

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
        /// Add startup task to <paramref name="services"/> as scoped
        /// </summary>
        public static IServiceCollection AddStartupTask<TTask>(this IServiceCollection services)
            where TTask : class, IStartupTask
        {
            return services.AddScoped<IBaseStartupTask, TTask>();
        }

        /// <summary>
        /// Add startup task to <paramref name="services"/> as scoped
        /// </summary>
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Type taskType)
        {
            return services.AddScoped(typeof(IBaseStartupTask), taskType);
        }

        /// <summary>
        /// Add sync startup task to <paramref name="services"/> as scoped
        /// </summary>
        public static IServiceCollection AddSyncStartupTask<TSyncTask>(this IServiceCollection services)
            where TSyncTask : class, ISyncStartupTask
        {
            return services.AddScoped<IBaseStartupTask, TSyncTask>();
        }

        /// <summary>
        /// Add sync startup task to <paramref name="services"/> as scoped
        /// </summary>
        public static IServiceCollection AddSyncStartupTask(this IServiceCollection services, Type taskType)
        {
            return services.AddScoped(typeof(IBaseStartupTask), taskType);
        }
        #endregion

        #region Add tasks from factory methods
        public static IServiceCollection AddStartupTask(this IServiceCollection services, Func<IServiceProvider, IStartupTask> factory)
        {
            return services.AddScoped<IBaseStartupTask>(factory);
        }
        #endregion
    }
}
