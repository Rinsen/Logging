using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Rinsen.Logger
{
    public static class LogExtensions
    {
        public static void AddLogger(this IServiceCollection services, Action<LogOptions> logOptionsAction)
        {
            var logOptions = new LogOptions();

            logOptionsAction.Invoke(logOptions);

            if (string.IsNullOrEmpty(logOptions.ConnectionString))
            {
                throw new ArgumentException("ConnectionString is mandatory", nameof(logOptions));
            }
            
            services.AddSingleton(logOptions);

            services.AddSingleton<ILogQueue, LogQueue>();
            services.AddSingleton<LogHandler, LogHandler>();
            services.AddSingleton<LoggerProvider, LoggerProvider>();
            services.AddTransient<DatabaseLogWriter, DatabaseLogWriter>();
        }

        public static ILoggerFactory UseLogger(this ILoggerFactory factory, IApplicationBuilder app)
        {
            factory.AddProvider(app.ApplicationServices.GetRequiredService<LoggerProvider>());

            Task.Run(() => StartLogger(app));

            return factory;
        }

        static LogHandler StartLogger(IApplicationBuilder app)
        {
            var logHandler = app.ApplicationServices.GetRequiredService<LogHandler>();

            logHandler.Start();

            return logHandler;
        }

        public static void UseLoggerDatabaseLogWriter(this IApplicationBuilder app)
        {
            var logHandler = app.ApplicationServices.GetRequiredService<LogHandler>();

            logHandler.LogWriters.Add(app.ApplicationServices.GetRequiredService<DatabaseLogWriter>());
        }
    }
}
