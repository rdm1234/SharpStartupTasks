using SharpStartupTasks.Abstractions.Configuration;
using System;

namespace SharpStartupTasks.Services
{
    public interface IStartupTaskConfigurationProvider
    {
        StartupConfigurationNode Get(IBaseStartupTask startupTask);
    }
}