using Microsoft.Extensions.DependencyInjection;
using System;
//using Microsoft.AspNetCore.Builder;

namespace Rinsen.Logger.Service
{
    public static class LogServiceExtensions
    {
        public static void AddLoggerService(this IServiceCollection services, Action<LogServiceOptions> logServiceOptionsAction)
        {
            var logOptions = new LogServiceOptions();

            logServiceOptionsAction.Invoke(logOptions);
                        
            services.AddSingleton(logOptions);
            services.AddSingleton<LogOptions>(logOptions);

            services.AddSingleton<ILogQueue, LogQueue>();
            services.AddSingleton<QueueLoggerProvider, QueueLoggerProvider>();
            services.AddSingleton<ILogServiceClient, LogServiceClient>();

            services.AddScoped<ILogReader, DatabaseLogReader>();
            services.AddScoped<ILogWriter, DatabaseLogWriter>();
            services.AddScoped<LogHandler, LogHandler>();
        }
    }
}
