using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rinsen.Logger
{
    public static class LogExtensions
    {
        public static void AddLogger(this IServiceCollection services, Action<LogOptions> logOptionsAction)
        {
            var logOptions = new LogOptions();

            logOptionsAction.Invoke(logOptions);
                        
            services.AddSingleton(logOptions);

            services.AddSingleton<ILogQueue, LogQueue>();
            services.AddSingleton<LogHandler, LogHandler>();
            services.AddSingleton<QueueLoggerProvider, QueueLoggerProvider>();
            services.AddSingleton<IRinsenLoggerInitializer, RinsenLoggerInitializer>();
            services.AddSingleton<ILogServiceClient, LogServiceClient>();
        }
    }
}
