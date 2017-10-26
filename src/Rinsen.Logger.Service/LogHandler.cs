using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Rinsen.Logger.Service
{
    public class LogHandler
    {
        private readonly ILogWriter _logWriter;
        private readonly ILogReader _logReader;

        public LogHandler(ILogReader logReader, ILogWriter logWriter)
        {
            _logReader = logReader;
            _logWriter = logWriter;
        }

        public async Task<bool> CreateLogs(LogReport logReport)
        {
            var logApplication = await _logReader.GetLogApplicationAsync(logReport.ApplicationKey);

            if (logApplication == default(LogApplication))
                return false;

            var logEnvironments = await _logReader.GetLogEnvironmentsAsync();

            var logs = new List<Log>();
            foreach (var log in logReport.LogItems)
            {
                var logEnvironment = logEnvironments.First(m => m.Name == log.EnvironmentName);

                logs.Add(new Log
                {
                    ApplicationId = logApplication.Id,
                    LogLevel = log.LogLevel,
                    EnvironmentId = logEnvironment.Id,
                    LogProperties = log.LogProperties,
                    MessageFormat = log.MessageFormat,
                    RequestId = log.RequestId,
                    SourceName = log.SourceName,
                    Timestamp = log.Timestamp
                });
            }

            await _logWriter.WriteLogsAsync(logs);

            return true;
        }
    }
}
