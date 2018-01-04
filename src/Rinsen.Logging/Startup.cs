using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Rinsen.IdentityProvider.Token;
using Rinsen.Logger.Service;
using Rinsen.DatabaseInstaller;
using System.Collections.Generic;
using Rinsen.Logger.Service.Installation;

namespace Rinsen.Logging
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTokenAuthentication(Configuration);

            services.AddLoggerService(options => 
            {
                options.ConnectionString = Configuration["Rinsen:ConnectionString"];
            });

            if (Env.IsDevelopment())
            {
                services.AddDatabaseInstaller(Configuration["Rinsen:ConnectionString"]);
            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminsOnly", policy => policy.RequireClaim("http://rinsen.se/Administrator"));
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.RunDatabaseInstaller(new List<DatabaseVersion>
                {
                    new CreateLogTable()
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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
