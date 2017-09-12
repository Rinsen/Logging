using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Rinsen.Logger
{
    public interface ILogQueue
    {
        void AddLog(string sourceName, string requestId, LogLevel logLevel, string messageFormat, IEnumerable<LogProperty> logProperties);
        IEnumerable<LogItem> GetReportedLogs();
    }
}
