using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Rinsen.Logger
{
    public interface ILogQueue
    {
        void AddLog(string sourceName, string environmentName, LogLevel logLevel, string message, string exceptionMessage, string stackTrace);
        bool IsEmpty();
        IEnumerable<LogItem> GetLogs();
    }
}
