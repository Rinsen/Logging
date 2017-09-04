using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Rinsen.Logger
{
    public static class LogExtensions
    {
        public static void AddRinsenLogger(this ILoggingBuilder loggingBuilder, Action<LogOptions> logOptionsAction)
        {
            var logOptions = new LogOptions();

            logOptionsAction.Invoke(logOptions);
                        
            loggingBuilder.Services.AddSingleton(logOptions);

            loggingBuilder.Services.AddSingleton<ILogQueue, LogQueue>();
            loggingBuilder.Services.AddSingleton<ILoggerProvider, QueueLoggerProvider>();
            loggingBuilder.Services.AddSingleton<ILogServiceClient, LogServiceClient>();
        }
    }
}
