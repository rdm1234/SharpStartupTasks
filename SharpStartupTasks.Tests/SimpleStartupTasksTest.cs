using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpStartupTasks.Extensions;
using SharpStartupTasks.Tests.SampleStartupTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStartupTasks.Tests;

public class SimpleStartupTasksTest : StartupTaskTestBase
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        base.ConfigureServices(services, configuration);

        services.AddSyncStartupTask<SimpleSynchronousStartupTask>();
        services.AddStartupTask<SimpleAsynchronousStartupTask>();
    }

    [Fact]
    public async Task SimpleRunTest()
    {
        await RunStartupTasksAsync();

        Assert.True(SimpleSynchronousStartupTask.Executed);
        Assert.True(SimpleAsynchronousStartupTask.Executed);
    }
}
