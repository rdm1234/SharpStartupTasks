using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;
using SharpStartupTasks.Extensions;
using System.Threading.Tasks;

namespace Examples.AspNetCoreWebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            /* If you want to log information about execution of startup tasks
             *  you should provide ILogger to the RunWithTasksAsync method. 
             *  In this example, SerilogLoggerFactory is used for this purpose. */
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var factory = new SerilogLoggerFactory(Log.Logger);

            var logger = factory.CreateLogger("SharpStartupTasks.HostExtensions");

            // Call RunWithTasksAsync extension method instead of Run or RunAsync
            await CreateHostBuilder(args).Build().RunWithTasksAsync(logger);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
