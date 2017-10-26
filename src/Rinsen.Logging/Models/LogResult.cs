using Microsoft.Extensions.Logging;
using Rinsen.Logger;
using Rinsen.Logger.Service;
using System;
using System.Collections.Generic;

namespace Rinsen.Logging.Models
{
    public class LogResult
    {
        private LogView _log;

        public LogResult(LogView log)
        {
            _log = log;
        }

        public int Id { get { return _log.Id; } }

        public string SourceName { get { return _log.SourceName; } }

        public string ApplicationName { get { return _log.ApplicationName; } }

        public string EnvironmentName { get { return _log.EnvironmentName; } }

        public string RequestId { get { return _log.RequestId; } }

        public LogLevel LogLevel { get { return _log.LogLevel; } }

        public string LogLevelName { get { return _log.LogLevel.ToString(); } }

        public string MessageFormat { get { return _log.MessageFormat; } }

        public string Message
        {
            get
            {  
                foreach (var property in _log.LogProperties)
                {
                    return _log.MessageFormat.Replace($"{{{property.Name}}}", property.Value);
                }
                return MessageFormat;
            }
        }

        public DateTimeOffset Timestamp { get { return _log.Timestamp; } }

        public IEnumerable<LogProperty> LogProperties { get { return _log.LogProperties; } }
    }
}
