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

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            
            string exceptionStackTrace;
            if (exception != null)
            {
                exceptionStackTrace = exception.StackTrace;
                ProcessInnerException(logLevel, exception);
            }
            else
            {
                exceptionStackTrace = string.Empty;
            }

            _logQueue.AddLog(_name, _environmentName, logLevel, formatter(state, exception), exceptionStackTrace);
        }

        private void ProcessInnerException(LogLevel logLevel, Exception exception)
        {
            if (exception.InnerException != null)
            {
                _logQueue.AddLog(_name, _environmentName, logLevel, exception.Message, exception.StackTrace);
                ProcessInnerException(logLevel, exception.InnerException);
            }
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
