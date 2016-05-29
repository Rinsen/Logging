using Microsoft.Extensions.Logging;
using System;

namespace Rinsen.Logger
{
    public class Logger : ILogger
    {
        readonly Func<string, LogLevel, bool> _filter;
        readonly string _name;
        readonly string _environmentName;
        readonly ILogQueue _logQueue;

        public Logger(string name, string environmentName, Func<string, LogLevel, bool> filter, ILogQueue logQueue)
        {
            _name = name;
            _filter = filter;
            _logQueue = logQueue;
            _environmentName = environmentName;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // If the filter is null, everything is enabled
            // unless the debugger is not attached
            return _filter == null || _filter(_name, logLevel);
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message;
            string exceptionStackTrace;
            if (state is string)
            {
                message = (string)state;
            }
            else
            {
                message = "No message";
            }
            exceptionStackTrace = formatter(state, exception);

            _logQueue.AddLog(_name, _environmentName, logLevel, message, exceptionStackTrace);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
