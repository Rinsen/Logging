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

        public void AddLog(string sourceName, string requestId, LogLevel logLevel, string messageFormat, IEnumerable<LogProperty> logProperties)
        {
            if (_logs.Count > _logOptions.QueueMazSize)
                return;

            _logs.Enqueue(new LogItem { SourceName = sourceName, EnvironmentName = _logOptions.EnvironmentName, RequestId = requestId, LogLevel = logLevel, MessageFormat = messageFormat, LogProperties = logProperties, Timestamp = DateTimeOffset.Now });
        }

        public IEnumerable<LogItem> GetReportedLogs()
        {
            if (_logs.IsEmpty)
            {
                return Enumerable.Empty<LogItem>();
            }

            int logCount = _logs.Count;
            var resultSize = logCount < _logOptions.MaxBatchSize ? logCount : _logOptions.MaxBatchSize;

            var result = new List<LogItem>(resultSize);

            for (int i = 0; i < resultSize; i++)
            {
                if (!_logs.TryDequeue(out LogItem logItem))
                    break;

                result.Add(logItem);
            }

            return result;
        }
    }
}
