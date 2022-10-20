using System;
using System.Collections.Generic;

namespace SharpStartupTasks.Abstractions.Configuration
{
    public class StartupTaskConfiguration : StartupConfigurationNode
    {
        /// <summary>
        /// Id helps to identify anonymous tasks
        /// </summary>
        public string? AnonymousTaskId { get; set; }

        /// <summary>
        /// Addition to <see cref="Name"/> allows to distinguish between multiple tasks with the same name.
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        /// Name of the task class
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Alternative to <see cref="Name"/> and <see cref="Namespace"/> pair to simplity configuration from code
        /// </summary>
        public Type? Type { get; set; }
    }
}