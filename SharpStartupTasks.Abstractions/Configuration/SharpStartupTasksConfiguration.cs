using System.Collections.Generic;

namespace SharpStartupTasks.Abstractions.Configuration
{
    public class SharpStartupTasksConfiguration
    {
        public StartupConfigurationNode? Global { get; set; }
        public StartupConfigurationNode? CurrentSet { get; set; }
        public List<StartupTaskConfiguration>? Tasks { get; set; }
    }
}