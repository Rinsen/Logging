using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Rinsen.Logger
{
    public static class LogExtensions
    {
        /// Add default configuration of Rinsen.Logger
        /// <para/>Connection string for storing session data and local user for reference ConnectionString
        /// <para/>Logging:LogApplicationKey
        /// <para/>Logging:LogServiceUrl
        /// <param name="loggingBuilder"></param>
        /// <param name="configuration"></param>
        /// <param name="environmentName"></param>
        public static void AddRinsenLogger(this ILoggingBuilder loggingBuilder, IConfiguration configuration, string environmentName)
        {
            loggingBuilder.AddRinsenLogger(options =>
            {
                options.ApplicationLogKey = configuration["Rinsen:ApplicationKey"];
                options.LogServiceUri = configuration["Rinsen:Logging:BaseUrl"].TrimEnd('/') + "/";
                options.EnvironmentName = environmentName;
            });
        }

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
