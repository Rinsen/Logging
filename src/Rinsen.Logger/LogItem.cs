using Microsoft.Extensions.Logging;
using System;

namespace Rinsen.Logger
{
    public class LogItem
    {
        public int Id { get; set; }

        public string SourceName { get; set; }

        public string EnvironmentName { get; set; }

        public LogLevel LogLevel { get; set; }

        public string LogLevelName { get { return LogLevel.ToString(); } }

        public string Message { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string StackTrace { get; set; }
    }
}
