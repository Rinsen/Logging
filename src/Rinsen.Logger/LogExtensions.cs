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
                throw new ArgumentException("Property ConnectionString is required", nameof(logOptions));
            }
            
            services.AddSingleton(logOptions);

            services.AddSingleton<ILogQueue, LogQueue>();
            services.AddSingleton<LogHandler, LogHandler>();
            services.AddSingleton<QueueLoggerProvider, QueueLoggerProvider>();
            services.AddSingleton<ILogWriter, DatabaseLogWriter>();
        }
    }
}
