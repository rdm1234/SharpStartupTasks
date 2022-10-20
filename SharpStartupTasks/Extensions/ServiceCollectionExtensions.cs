using Microsoft.Extensions.DependencyInjection;
using SharpStartupTasks.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds startup task runner
        /// </summary>
        /// <param name="services"></param>
        public static void AddSharpStartupTasksCore(this IServiceCollection services)
        {
            services.AddScoped<IStartupTasksRunner, StartupTasksRunner>();
            services.AddScoped<IStartupTaskConfigurationProvider, StartupTaskConfigurationProvider>();
        }
    }
}