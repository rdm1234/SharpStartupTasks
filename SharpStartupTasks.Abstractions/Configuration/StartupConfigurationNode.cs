using System;
using System.Collections.Generic;

namespace SharpStartupTasks.Abstractions.Configuration
{
    public class StartupConfigurationNode
    {
        public bool? Disabled { get; set; }
        public bool? IgnoreAllExceptions { get; set; }
        public List<string>? IgnoreExceptions { get; set; }
        public string[]? EnabledEnvironments { get; set; }
        public Type[]? IgnoreExceptionTypes { get; set; }
    }
}