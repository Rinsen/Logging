using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Rinsen.Logger;
using Microsoft.Extensions.Logging;

namespace LoggerSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddEnvironmentVariables();
                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Startup>();
                    }
                })
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder
                        .AddFilter("Microsoft", LogLevel.Debug)
                        .AddFilter("System", LogLevel.Debug)
                        .AddFilter("LoggerSample", LogLevel.Debug)
                        .AddRinsenLogger(options => {
                            options.MinLevel = LogLevel.Debug;
                            options.ApplicationLogKey = hostingContext.Configuration["Logging:LogApplicationKey"];
                            options.LogServiceUri = hostingContext.Configuration["Logging:Uri"];
                            options.EnvironmentName = hostingContext.HostingEnvironment.EnvironmentName;
                        });
                })
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}
