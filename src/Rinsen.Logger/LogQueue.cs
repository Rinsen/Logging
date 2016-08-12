using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rinsen.Logger
{
    public class LogQueue : ILogQueue
    {
        readonly ConcurrentQueue<LogItem> _logs;
        readonly LogOptions _logOptions;

        public LogQueue(LogOptions logOptions)
        {
            _logs = new ConcurrentQueue<LogItem>();
            _logOptions = logOptions;
        }

        public bool IsEmpty()
        {
            return _logs.Count < 1;
        }

        public void AddLog(string sourceName, string environmentName, LogLevel logLevel, string message, string exceptionMessage, string stackTrace)
        {
            if (_logs.Count > _logOptions.QueueMazSize)
                return;

            _logs.Enqueue(new LogItem { SourceName = sourceName, EnvironmentName = environmentName, LogLevel = logLevel, Message = message, Timestamp = DateTimeOffset.Now, ExceptionMessage = exceptionMessage, StackTrace = stackTrace });
        }

        public IEnumerable<LogItem> GetLogs()
        {
            if (_logs.IsEmpty)
            {
                return Enumerable.Empty<LogItem>();
            }

            int logCount = _logs.Count;
            var resultSize = logCount < _logOptions.MaxBatchSize ? logCount : _logOptions.MaxBatchSize;

            var result = new List<LogItem>(resultSize);

            LogItem logItem;
            for (int i = 0; i < resultSize; i++)
            {
                if (!_logs.TryDequeue(out logItem))
                    break;

                result.Add(logItem);
            }

            return result;
        }
    }
}
