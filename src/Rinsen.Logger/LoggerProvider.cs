using Microsoft.Extensions.Logging;
using System;

namespace Rinsen.Logger
{
    public class LoggerProvider : ILoggerProvider
    {
        readonly Func<string, LogLevel, bool> _filter;
        readonly ILogQueue _logQueue;
        readonly LogOptions _options;

        public LoggerProvider(ILogQueue logQueue, LogOptions options)
        {
            _filter = (category, logLevel) => logLevel >= options.MinLevel && category.StartsWith("");
            _logQueue = logQueue;
            _options = options;
        }

        public ILogger CreateLogger(string name)
        {
            return new Logger(name, _options.EnvironmentName, _filter, _logQueue);
        }

        public void Dispose()
        {
            // TODO what to dispose?
        }
    }
}
