using Microsoft.Extensions.Logging;
using Rinsen.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logging.Models
{
    public class LogResult
    {
        public int Id { get; set; }

        public string SourceName { get; set; }

        public string ApplicationName { get; set; }

        public string EnvironmentName { get; set; }

        public string RequestId { get; set; }

        public LogLevel LogLevel { get; set; }

        public string MessageFormat { get; set; }

        public string Message { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public IEnumerable<LogProperty> LogProperties { get; set; }
    }
}
