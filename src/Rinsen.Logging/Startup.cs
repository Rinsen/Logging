using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Rinsen.Logger.Service;
using Rinsen.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Rinsen.IdentityProvider.Token;
using Rinsen.IdentityProvider.Core;

namespace Rinsen.Logging
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddLoggerService(options =>
            {
                options.ConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
                options.MinLevel = LogLevel.Debug;
                options.ApplicationLogKey = Configuration["Logging:LogApplicationKey"];
                options.LogServiceUri = Configuration["Logging:Uri"];
                options.EnvironmentName = _env.EnvironmentName;
            });

            services.AddRinsenAuthentication();

            services.AddAuthorization(options =>
            {

                //options.AddPolicy("AlwaysFail", policy => policy.Requirements.Add(new AlwaysFailRequirement()));

            });

            services.AddMvc(config =>
            {
                config.Filters.Add(new RequireHttpsAttribute());

                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();

                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IRinsenLoggerInitializer logInitializer)
        {
            logInitializer.Run(new FilterLoggerSettings {
                { "Microsoft", LogLevel.Information },
                { "Rinsen", LogLevel.Information }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseTokenAuthenticationWithCookieAuthentication(new TokenOptions(Configuration["Data:DefaultConnection:ConnectionString"])
            {
                ApplicationKey = Configuration["IdentityProvider:ApplicationKey"],
                LoginPath = Configuration["IdentityProvider:LoginPath"],
                ValidateTokenPath = Configuration["IdentityProvider:ValidateTokenPath"]
            },
                new RinsenDefaultCookieAuthenticationOptions(Configuration["Data:DefaultConnection:ConnectionString"])
            );

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
