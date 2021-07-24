using Examples.AspNetCoreWebApi.Services;
using Examples.AspNetCoreWebApi.StartupTasks;
using Examples.SeparatedStartupTasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharpStartupTasks.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examples.AspNetCoreWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Examples.AspNetCoreWebApi", Version = "v1" });
            });

            services.AddSingleton<MainStartupDependentService>();
            services.AddSingleton<SomeStartupDependentService>();

            services.AddStartupTasksFromAssembleyOf<Startup>(
                typeof(MustRunAfterAllStartupTask), 
                typeof(SecondMustRunAfterAllStartupTask));

            services.AddStartupTasksFromAssembleyThenOrdered<SomeStartupDependentService>(
                orderedStartupTasks: new[] { typeof(MustRunAfterAllSeparetedStartupTask) },
                exceptTypes: new[] { typeof(ShouldNotRunForNowSeparetedStartupTask) });

            services.AddStartupTask<MustRunAfterAllStartupTask>();
            services.AddStartupTask<SecondMustRunAfterAllStartupTask>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Examples.AspNetCoreWebApi v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
