using Microsoft.Extensions.DependencyInjection;
using SharpStartupTasks.Abstractions.Configuration;
using SharpStartupTasks.Extensions;
using SharpStartupTasks.Services;
using SharpStartupTasks.Tests.SampleStartupTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Tests;

public class ConfigurationProviderTests : StartupTaskTestBase
{
    private IStartupTaskConfigurationProvider ConfigurationProvider => ServiceProvider.GetRequiredService<IStartupTaskConfigurationProvider>();

    [Fact]
    public void NoConfigTest()
    {
        var globalCfg = ((StartupTaskConfigurationProvider)ConfigurationProvider).GlobalConfig;

        Assert.False(globalCfg.Disabled);
        Assert.False(globalCfg.IgnoreAllExceptions);
        Assert.Null(globalCfg.IgnoreExceptions);
        Assert.Null(globalCfg.IgnoreExceptionTypes);
        Assert.Null(globalCfg.EnabledEnvironments);
    }

    [Theory]
    [InlineData(new object[] { null, null, null, null, null })]
    [InlineData(new object[] { true, true, new string[] {  }, new Type[] {}, new string[] { } })]
    public void GlobalConfigTest(bool? disabled, bool? ignoreAllExceptions, string[]? ignoreExceptionsParameter, Type[]? ignoreExceptionTypes, string[]? enabledEnvironments)
    {
        var ignoreExceptions = ignoreExceptionsParameter?.ToList();
        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                Global = new()
                {
                    Disabled = disabled,
                    IgnoreAllExceptions = ignoreAllExceptions,
                    IgnoreExceptions = ignoreExceptions,
                    IgnoreExceptionTypes = ignoreExceptionTypes,
                    EnabledEnvironments = enabledEnvironments
                }
            });
        });

        var globalCfg = ((StartupTaskConfigurationProvider)sp.GetRequiredService<IStartupTaskConfigurationProvider>()).GlobalConfig;
        
        Assert.Equal(disabled ?? false, globalCfg.Disabled);
        Assert.Equal(ignoreAllExceptions ?? false, globalCfg.IgnoreAllExceptions);
        Assert.Equal(ignoreExceptions, globalCfg.IgnoreExceptions);
        Assert.Equal(ignoreExceptionTypes, globalCfg.IgnoreExceptionTypes);
        Assert.Equal(enabledEnvironments, globalCfg.EnabledEnvironments);
    }

    [Fact]
    public void ContextConfigTest()
    {
        var globalCfg = new StartupConfigurationNode()
        {
            Disabled = true,
            IgnoreAllExceptions = false,
            EnabledEnvironments = new[] { "Dev" },
            IgnoreExceptions = new() { nameof(Exception) },
            IgnoreExceptionTypes = new[] { typeof(Exception) }
        };

        var ctxCfg = new StartupConfigurationNode()
        {
            Disabled = false,
            IgnoreAllExceptions = true,
            EnabledEnvironments = new[] { "Prod " },
            IgnoreExceptions = new() { nameof(ArgumentException) },
            IgnoreExceptionTypes = new[] { typeof(ArgumentException) }
        };

        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                Global = globalCfg,
                CurrentSet = ctxCfg,
                Tasks = new() { new() { Type = typeof(SimpleAsynchronousStartupTask) } }
            });
        });

        var cfgProvider = sp.GetRequiredService<IStartupTaskConfigurationProvider>();

        var cfg = cfgProvider.Get(new SimpleAsynchronousStartupTask());

        Assert.Equal(ctxCfg.Disabled, cfg.Disabled);
        Assert.Equal(ctxCfg.IgnoreAllExceptions, cfg.IgnoreAllExceptions);
        Assert.Equal(ctxCfg.EnabledEnvironments, cfg.EnabledEnvironments);
        Assert.Equal(ctxCfg.IgnoreExceptions, cfg.IgnoreExceptions);
        Assert.Equal(ctxCfg.IgnoreExceptionTypes, cfg.IgnoreExceptionTypes);
    }

    [Fact]
    public void TaskConfigTest()
    {
        var globalCfg = new StartupConfigurationNode()
        {
            Disabled = true,
            IgnoreAllExceptions = false,
            EnabledEnvironments = new[] { "Dev" },
            IgnoreExceptions = new() { nameof(Exception) },
            IgnoreExceptionTypes = new[] { typeof(Exception) }
        };

        var ctxCfg = new StartupConfigurationNode()
        {
            Disabled = true,
            IgnoreAllExceptions = false,
            EnabledEnvironments = new[] { "Prod" },
            IgnoreExceptions = new() { nameof(ArgumentException) },
            IgnoreExceptionTypes = new[] { typeof(ArgumentException) }
        };

        var taskCfg = new StartupTaskConfiguration()
        {
            Type = typeof(SimpleAsynchronousStartupTask),
            Disabled = false,
            IgnoreAllExceptions = true,
            EnabledEnvironments = new[] { "Stage" },
            IgnoreExceptions = new() { nameof(DivideByZeroException) },
            IgnoreExceptionTypes = new[] { typeof(DivideByZeroException) }
        };

        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                Global = globalCfg,
                CurrentSet = ctxCfg,
                Tasks = new() { taskCfg }
            });
        });
        
        var cfgProvider = sp.GetRequiredService<IStartupTaskConfigurationProvider>();

        var cfg = cfgProvider.Get(new SimpleAsynchronousStartupTask());

        Assert.Equal(taskCfg.Disabled, cfg.Disabled);
        Assert.Equal(taskCfg.IgnoreAllExceptions, cfg.IgnoreAllExceptions);
        Assert.Equal(taskCfg.EnabledEnvironments, cfg.EnabledEnvironments);
        Assert.Equal(taskCfg.IgnoreExceptions, cfg.IgnoreExceptions);
        Assert.Equal(taskCfg.IgnoreExceptionTypes, cfg.IgnoreExceptionTypes);
    }
}
