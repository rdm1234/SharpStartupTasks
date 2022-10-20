using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using SharpStartupTasks.Extensions;
using SharpStartupTasks.Services;

namespace SharpStartupTasks.Tests;

public class StartupTaskTestBase
{
	public StartupTaskTestBase()
	{
		ServiceProvider = BuildServiceProvider(out var cfg, ConfigureServices, GetConfiguration, Env);
		Configuration = cfg;
	}
    
	protected ServiceProvider BuildServiceProviderWithStartupRunner(Action<IServiceCollection, IConfiguration> configureService, Func<HostingEnvironment, string, IConfigurationRoot>? provideConfiguration = null, string? env = null)
	{
        Action<IServiceCollection, IConfiguration> newConfigureServices = (services, cfg) =>
        {
            services.AddSharpStartupTasksCore();
            configureService(services, cfg);
        };

        return BuildServiceProvider(out _, newConfigureServices, provideConfiguration, env);
	}
    
	protected ServiceProvider BuildServiceProvider(Action<IServiceCollection, IConfiguration> configureService, Func<HostingEnvironment, string, IConfigurationRoot>? provideConfiguration = null, string? env = null)
	{
		return BuildServiceProvider(out _, configureService, provideConfiguration, env);
	}
    
	protected ServiceProvider BuildServiceProvider(out IConfigurationRoot cfg, Action<IServiceCollection, IConfiguration> configureService, Func<HostingEnvironment, string, IConfigurationRoot>? provideConfiguration = null, string? env = null)
	{
        if (provideConfiguration == null)
		{
			provideConfiguration = GetConfiguration;
		}

        if (configureService == null)
		{
			configureService = ConfigureServices;
		}

        if (env == null)
		{
			env = Env;
		}

		var hostEnv = new HostingEnvironment();
		hostEnv.ContentRootPath = Directory.GetCurrentDirectory();
		hostEnv.EnvironmentName = env;
		Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);

		var sc = new ServiceCollection();

		cfg = provideConfiguration(hostEnv, env);
        
		configureService(sc, cfg);

		var sp = sc.BuildServiceProvider();

		return sp;
	}

	protected static IConfigurationRoot GetConfiguration(HostingEnvironment hostEnv, string env)
	{
		return new ConfigurationBuilder()
			.SetBasePath(hostEnv.ContentRootPath)
			.AddJsonFile("appsettings.json", optional: false)
			.AddJsonFile($"appsettings.{env}.json", optional: true)
			.AddEnvironmentVariables()
			.Build();
	}

	public virtual string Env => "Default";

    public ServiceProvider ServiceProvider { get; }
	public IConfigurationRoot Configuration { get; }

    /// <summary>
    /// Add services to <paramref name="services"/>. By default calls <see cref="ServiceCollectionExtensions.AddSharpStartupTasksCore(IServiceCollection)"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) 
	{
		services.AddSharpStartupTasksCore();
    }
    
    public static async Task RunStartupTasksAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var runner = serviceProvider.GetRequiredService<IStartupTasksRunner>();
        await runner.RunTasksAsync(cancellationToken);
    }

    public async Task RunStartupTasksAsync(CancellationToken cancellationToken = default)
    {
        await RunStartupTasksAsync(ServiceProvider, cancellationToken);
    }
}