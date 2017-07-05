using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.Logger.Service
{
    public class LogView
    {
        public int Id { get; set; }

        public string SourceName { get; set; }

        public string ApplicationName { get; set; }

        public string EnvironmentName { get; set; }

        public string RequestId { get; set; }

        public LogLevel LogLevel { get; set; }

        public string MessageFormat { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public IEnumerable<LogProperty> LogProperties { get; set; }
    }
}