using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpStartupTasks.Abstractions.Configuration;
using SharpStartupTasks.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Tests;

public class AnonymousStartupTaskTest : StartupTaskTestBase
{
    private bool FirstAnonymousTaskExecuted { get; set; }
    private bool SecondAnonymousTaskExecuted { get; set; }

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        base.ConfigureServices(services, configuration);

        services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
        {
            Global = new()
            {
                IgnoreExceptionTypes = new[] { typeof(InvalidOperationException) }
            },
            Tasks = new()
            {
                new()
                {
                    AnonymousTaskId = "some-task-3",
                    IgnoreExceptionTypes = new[] { typeof(DivideByZeroException) }
                }
            }
        });

        services.AddSyncStartupTask(() =>
        {
            throw new Exception("Sample exception from first startup task");
        }, new StartupConfigurationNode()
        {
            IgnoreAllExceptions = true
        });

        services.AddStartupTask(() =>
        {
            FirstAnonymousTaskExecuted = true;
            return Task.CompletedTask;
        }, new StartupConfigurationNode()
        {
            Disabled = false
        });

        services.AddSyncStartupTask(() =>
        {
            throw new DivideByZeroException("Sample exception from third startup task");
        }, "some-task-3");
        
        services.AddSyncStartupTask(() =>
        {
            throw new InvalidOperationException("Sample exception from fourth startup task");
        });
        
        services.AddSyncStartupTask(() =>
        {
            FirstAnonymousTaskExecuted = true;
        }, new StartupConfigurationNode()
        {
            Disabled = true
        });

        services.AddSyncStartupTask(() =>
        {
            throw new ArgumentOutOfRangeException("Sample should not be caught exception from the last task");
        });
    }

    [Fact]
    public async void AnonymousTasksTest()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await RunStartupTasksAsync(ServiceProvider);
        });

        Assert.True(FirstAnonymousTaskExecuted);
        Assert.False(SecondAnonymousTaskExecuted);
    }
}
