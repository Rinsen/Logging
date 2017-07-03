using Microsoft.AspNetCore.Mvc;
using Rinsen.Logger;
using System.Threading.Tasks;
using Rinsen.Logger.Service;
using System.Linq;
using System.Collections.Generic;
using Rinsen.Logging.Models;

namespace Rinsen.Logging.Controllers
{
    public class LoggerController : Controller, ILogServiceClient
    {
        private readonly ILogWriter _logWriter;
        private readonly ILogReader _logReader;

        public LoggerController(ILogReader logReader, ILogWriter logWriter)
        {
            _logReader = logReader;
            _logWriter = logWriter;
        }

        public async Task<IActionResult> Index()
        {
            var model = new LoggerModel
            {
                SelectionOptions = new SelectionOptions
                {
                    LogApplications = (await _logReader.GetLogApplicationsAsync()).Select(m => new SelectionLogApplication { Id = m.Id, ApplicationName = m.ApplicationName }),
                    LogEnvironments = await _logReader.GetLogEnvironmentsAsync(),
                    LogLevels = GetLogLevels()
                }
            };

            return View(model);
        }

        private IEnumerable<SelectionLogLevel> GetLogLevels()
        {
            return new List<SelectionLogLevel>
            {
                new SelectionLogLevel { Level = 0, Name = "Trace"},
                new SelectionLogLevel { Level = 1, Name = "Debug"},
                new SelectionLogLevel { Level = 2, Name = "Information"},
                new SelectionLogLevel { Level = 3, Name = "Warning"},
                new SelectionLogLevel { Level = 4, Name = "Error"},
                new SelectionLogLevel { Level = 5, Name = "Critical"},

            };
        }

        [HttpPost]
        public async Task<bool> ReportAsync([FromBody]LogReport logReport)
        {
            var logApplication = await _logReader.GetLogApplicationAsync(logReport.ApplicationKey);

            if (logApplication == default(LogApplication))
                return false;

            var logEnvironments = await GetLogEnvironments(logReport);

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

        private async Task<List<LogEnvironment>> GetLogEnvironments(LogReport logReport)
        {
            var environmentNames = logReport.LogItems.Select(m => m.EnvironmentName).Distinct();

            var logEnvironments = await _logReader.GetLogEnvironmentsAsync();

            foreach (var environmentName in environmentNames)
            {
                if(!logEnvironments.Any(m => m.Name == environmentName))
                {
                    logEnvironments.Add(await _logWriter.CreateLogEnvironmentAsync(environmentName));
                }
            }

            return logEnvironments;
        }
    }
}
