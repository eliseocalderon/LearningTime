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
using LearningTime.Models;
using Microsoft.Framework.Logging;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using LearningTime.ViewModels;

namespace LearningTime
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static IHostingEnvironment Env { get; set; }

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
            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            services.AddLogging();
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<LearningContext>();
            services.AddScoped<CoordService>();
            services.AddTransient<LearningContextSeedData>();
            services.AddScoped<ILearningRepository, LearningRepository>();
            if (Env.IsDevelopment())
            {
                services.AddScoped<IMailService, DebugMailService>();
            }
        }

        public void Configure(IApplicationBuilder app, LearningContextSeedData seeder, ILoggerFactory loggerFactory)
        {
            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();

            loggerFactory.AddDebug(LogLevel.Warning);

            app.UseStaticFiles();

            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>().ReverseMap();
                config.CreateMap<Stop, StopViewModel>().ReverseMap();
            });

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

            seeder.EnsureSeeData();
        }
    }
}
