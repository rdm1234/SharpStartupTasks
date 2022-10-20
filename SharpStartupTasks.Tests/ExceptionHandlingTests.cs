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

public class ExceptionHandlingTests : StartupTaskTestBase
{
    [Fact]
    public async Task DefaultExceptionHandlingTest()
    {
        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddSyncStartupTask<ThrowingExceptionStartupTask>();
        });

        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await RunStartupTasksAsync(sp);
        });
    }

    [Fact]
    public async Task IgnoreAllExceptionsTest()
    {
        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddSyncStartupTask<ThrowingExceptionStartupTask>();
            services.AddSyncStartupTask<ThrowingExceptionStartupTask<SomeException1>>();

            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                Global = new()
                {
                    IgnoreAllExceptions = true
                }
            });
        });

        await RunStartupTasksAsync(sp);
    }
    
    [Fact]
    public async Task IgnoreAllExceptionsByListTest()
    {
        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddSyncStartupTask<ThrowingExceptionStartupTask>();
            services.AddSyncStartupTask<ThrowingExceptionStartupTask<SomeException1>>();

            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                Global = new() 
                { 
                    IgnoreExceptions = new() { nameof(Exception), nameof(SomeException1) }
                }
            });
        });

        await RunStartupTasksAsync(sp);
    }
    
    [Fact]
    public async Task IgnorePartOfExceptionsTest()
    {
        var sp = BuildServiceProviderWithStartupRunner((services, cfg) =>
        {
            services.AddSyncStartupTask<ThrowingExceptionStartupTask>();
            services.AddSyncStartupTask<ThrowingExceptionStartupTask<SomeException1>>();
            services.AddSyncStartupTask<ThrowingExceptionStartupTask<SomeException2>>();

            services.AddStartupTasksConfiguration(new SharpStartupTasksConfiguration()
            {
                Global = new()
                {
                    IgnoreExceptions = new() { nameof(Exception), nameof(SomeException1) }
                }
            });
        });

        await Assert.ThrowsAsync<SomeException2>(async () =>
        {
            await RunStartupTasksAsync(sp);
        });
    }

    private class SomeException1 : Exception
    {
        
    }

    private class SomeException2 : Exception
    {
        
    }
}
