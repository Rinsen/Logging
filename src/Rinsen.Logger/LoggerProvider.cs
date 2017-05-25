using Microsoft.Extensions.Logging;
using System;

namespace Rinsen.Logger
{
    public class QueueLoggerProvider : ILoggerProvider
    {
        readonly Func<string, LogLevel, bool> _filter;
        readonly ILogQueue _logQueue;
        readonly LogOptions _options;

        public QueueLoggerProvider(ILogQueue logQueue, LogOptions options)
        {
            _logQueue = logQueue;
            _options = options;
            _filter = (category, logLevel) => logLevel >= options.MinLevel && category.StartsWith("");
        }

        public ILogger CreateLogger(string name)
        {
            return new Logger(name, _filter, _logQueue);
        }

        public void Dispose()
        {
        }
    }
}
