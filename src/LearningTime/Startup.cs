using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using LearningTime.Services;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Dnx.Runtime;

namespace LearningTime
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;
        public static IHostingEnvironment Env;

        public Startup(IApplicationEnvironment appEnv, IHostingEnvironment env)
        {
            Env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();

        }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            if (Env.IsDevelopment())
            {
                services.AddScoped<IMailService, DebugMailService>();
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();
            //app.UseMvcWithDefaultRoute();
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new
                    {
                        controller = "App",
                        action = "Index"
                    }
                    );
            });
            app.UseStaticFiles();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
