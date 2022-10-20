using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpStartupTasks.Abstractions.Configuration;
using SharpStartupTasks.Extensions;
using SharpStartupTasks.Tests.SampleStartupTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Tests;

public class EnabledEnvironmentsTests : StartupTaskTestBase
{    
    [Theory]
    [InlineData("Dev", true, false)]
    [InlineData("Prod", false, true)]
    [InlineData("Stage", true, true)]
    [InlineData("Other", false, false)]
    public async void EnabledEnvironmentsTest(string env, bool simpleStartupTaskExcecuted, bool anonymousStartupTaskExecuted)
    {
        bool anonymousTaskExecuted = false;
        
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);
        
        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                Global = new()
                {
                    EnabledEnvironments = new[] { "Stage", "Dev" }
                }
            });

            services.AddStartupTask<SimpleAsynchronousStartupTask>();
            services.AddSyncStartupTask(() =>
            {
                anonymousTaskExecuted = true;
            }, new StartupConfigurationNode
            {
                EnabledEnvironments = new[] { "Prod", "Stage" }
            });
        }, null, env);

        SimpleAsynchronousStartupTask.Executed = false;
        
        await RunStartupTasksAsync(sp);
        
        Assert.Equal(simpleStartupTaskExcecuted, SimpleAsynchronousStartupTask.Executed);
        Assert.Equal(anonymousStartupTaskExecuted, anonymousTaskExecuted);
    }
}
