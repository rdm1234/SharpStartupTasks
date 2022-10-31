using Microsoft.Extensions.DependencyInjection;
using SharpStartupTasks.Abstractions.Configuration;
using SharpStartupTasks.Extensions;
using SharpStartupTasks.Services;

namespace SharpStartupTasks.Tests;

public class AnonymousTasksOverloadsTests : StartupTaskTestBase
{
    [Fact]
    public async Task FullOverloadsTest()
    {
        bool firstExecuted = false;
        bool secondExecuted = false;
        bool thirdExecuted = false;
        bool fourthExecuted = false;
        
        var sp = BuildServiceProviderWithStartupRunner((services, configuration) =>
        {
            services.AddStartupTask((sp, ct) =>
            {
                var cfgProvider = sp.GetRequiredService<IStartupTaskConfigurationProvider>();
                firstExecuted = true;
                return Task.CompletedTask;
            });
            
            services.AddStartupTask((sp, ct) =>
            {
                secondExecuted = true;
                throw new Exception("Sample exception");
            }, new StartupConfigurationNode
            {
                IgnoreAllExceptions = true
            });

            services.AddSyncStartupTask((sp, ct) =>
            {
                var cfgProvider = sp.GetRequiredService<IStartupTaskConfigurationProvider>();
                thirdExecuted = true;
            });

            services.AddSyncStartupTask((sp, ct) =>
            {
                fourthExecuted = true;
                throw new Exception("Sample exception");
            }, new StartupConfigurationNode
            {
                IgnoreAllExceptions = true
            });
        });

        await RunStartupTasksAsync(sp);

        Assert.True(firstExecuted);
        Assert.True(secondExecuted);
        Assert.True(thirdExecuted);
        Assert.True(fourthExecuted);
    }

    [Fact]
    public async Task ProvideCancellationTokenTest()
    {
        bool firstExecuted = false;
        bool secondExecuted = false;
        bool thirdExecuted = false;
        bool fourthExecuted = false;

        var sp = BuildServiceProviderWithStartupRunner((services, configuration) =>
        {
            services.AddStartupTask(ct =>
            {
                ct.ThrowIfCancellationRequested();
                firstExecuted = true;
                return Task.CompletedTask;
            });
            
            services.AddStartupTask(ct =>
            {
                secondExecuted = true;
                throw new Exception("Some exception");
            }, new StartupConfigurationNode
            {
                IgnoreAllExceptions = true
            });

            services.AddSyncStartupTask(ct =>
            {
                ct.ThrowIfCancellationRequested();
                thirdExecuted = true;
            });

            services.AddSyncStartupTask(ct =>
            {
                fourthExecuted = true;
                throw new Exception("Some exception");
            }, new StartupConfigurationNode
            {
                IgnoreAllExceptions = true
            });

            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration
            {
                Global = new StartupConfigurationNode()
                {
                    IgnoreExceptionTypes = new[] { typeof(OperationCanceledException) }
                }
            });
        });

        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        cts.Cancel();

        await RunStartupTasksAsync(sp, ct);

        Assert.False(firstExecuted);
        Assert.True(secondExecuted);
        Assert.False(firstExecuted);
        Assert.True(secondExecuted);
    }

    [Fact]
    public async Task NoParamsOverloadTest()
    {
        bool firstExecuted = false;
        bool secondExecuted = false;
        bool thirdExecuted = false;
        bool fourthExecuted = false;
        
        var sp = BuildServiceProviderWithStartupRunner((services, configuration) =>
        {
            services.AddStartupTask(() =>
            {
                firstExecuted = true;
                return Task.CompletedTask;
            });
            
            services.AddStartupTask(() =>
            {
                secondExecuted = true;
                throw new Exception("Some exception");
            }, new StartupConfigurationNode
            {
                IgnoreAllExceptions = true
            });

            services.AddSyncStartupTask(() =>
            {
                thirdExecuted = true;
            });
            
            services.AddSyncStartupTask(() =>
            {
                fourthExecuted = true;
                throw new Exception("Some exception");
            }, new StartupConfigurationNode
            {
                IgnoreAllExceptions = true
            });
        });

        await RunStartupTasksAsync(sp);

        Assert.True(firstExecuted);
        Assert.True(secondExecuted);
        Assert.True(thirdExecuted);
        Assert.True(fourthExecuted);
    }
}