using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Rinsen.Logger.Service
{
    public class Log
    {
        public int Id { get; set; }

        public string SourceName { get; set; }

        public int ApplicationId { get; set; }

        public int EnvironmentId { get; set; }

        public string RequestId { get; set; }

        public LogLevel LogLevel { get; set; }

        public string MessageFormat { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public IEnumerable<LogProperty> LogProperties { get; set; }
    }
}
