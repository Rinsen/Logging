﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rinsen.Logger;

namespace LoggerSample
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Env { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddLogger(options =>
            {
                //options.ConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
                options.EnvironmentName = Env.EnvironmentName;
                options.MinLevel = LogLevel.Debug;
                options.ApplicationLogKey = "Hello123";
                options.LogServiceUri = "http://localhost:61904/";
                //options.Application = "LoggerSample";
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IRinsenLoggerInitializer logInitializer)
        {
            logInitializer.Run(new FilterLoggerSettings {
                { "Microsoft", LogLevel.Debug },
                { "Rinsen", LogLevel.Debug }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseLogMiddleware();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var logger = loggerFactory.CreateLogger<Startup>();

            logger.LogDebug("Staring things");
        }
    }
}
