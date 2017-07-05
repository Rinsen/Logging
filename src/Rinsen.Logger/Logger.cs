using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rinsen.Logger
{
    public class Logger : ILogger
    {
        readonly Func<string, LogLevel, bool> _filter;
        readonly string _name;
        readonly ILogQueue _logQueue;

        public Logger(string name, Func<string, LogLevel, bool> filter, ILogQueue logQueue)
        {
            _name = name;
            _filter = filter;
            _logQueue = logQueue;
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

            if (state is IEnumerable<KeyValuePair<string, object>> keyValuePairs)
            {
                var logProperties = new List<LogProperty>();
                string messageTemplate = string.Empty;
                string requestId = string.Empty;

                foreach (var keyValue in keyValuePairs)
                {
                    if (keyValue.Key == "{OriginalFormat}" && keyValue.Value is string)
                    {
                        messageTemplate = (string)keyValue.Value;
                    }
                    else
                    {
                        var value = keyValue.Value?.ToString();
                        logProperties.Add(new LogProperty { Name = keyValue.Key, Value = value ?? "null" });
                    }
                }

                var current = RinsenLogScope.Current;
                while (current != null)
                {
                    foreach (var keyValue in current.GetScopeKeyValuePairs())
                    {
                        if (keyValue.Key == "RequestId" && keyValue.Value is string)
                        {
                            requestId = (string)keyValue.Value;
                        }
                        else
                        {
                            logProperties.Add(new LogProperty { Name = keyValue.Key, Value = keyValue.Value.ToString() });
                        }
                    }
                    
                    current = current.Parent;
                }

                AddExceptionInformation(exception, logProperties);

                _logQueue.AddLog(_name, requestId, logLevel, messageTemplate, logProperties);
            }
            else
            {
                var requestId = GetRequestId();
                _logQueue.AddLog(_name, requestId, logLevel, formatter(state, exception), Enumerable.Empty<LogProperty>());
            }
        }

        private void AddExceptionInformation(Exception exception, List<LogProperty> logProperties, int count = 0)
        {
            if (exception != null)
            {
                logProperties.Add(new LogProperty { Name = $"ExceptionMessage_{count}", Value = exception.Message });
                logProperties.Add(new LogProperty { Name = $"ExceptionStackTrace_{count}", Value = exception.StackTrace });

                AddExceptionInformation(exception.InnerException, logProperties);
            }
        }

        private static string GetRequestId()
        {
            var current = RinsenLogScope.Current;
            while (current != null)
            {
                var scopeItems = current.GetScopeKeyValuePairs();
                var requestId = scopeItems.FirstOrDefault(m => m.Key == "RequestId");

                if (!requestId.Equals(default(KeyValuePair<string, object>)))
                {
                    return requestId.Value.ToString();
                }

                current = current.Parent;
            }

            return string.Empty;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return RinsenLogScope.Push(_name, state);
        }
    }
}
