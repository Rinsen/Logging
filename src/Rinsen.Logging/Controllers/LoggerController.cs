using Microsoft.AspNetCore.Mvc;
using Rinsen.Logger;
using System.Threading.Tasks;
using Rinsen.Logger.Service;
using System.Linq;
using System.Collections.Generic;
using Rinsen.Logging.Models;
using System;

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
                    LogApplications = await GetLogApplications(),
                    LogEnvironments = await GetLogEnvironments(),
                    LogLevels = GetLogLevels(),
                    From = DateTimeOffset.Now.AddHours(-24),
                    To = DateTimeOffset.Now.AddMinutes(10)
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IEnumerable<LogResult>> GetLogs([FromBody]SearchModel searchModel)
        {
            var logViews = await _logReader.GetLogs(searchModel.From, searchModel.To, searchModel.LogApplications, searchModel.LogEnvironments, searchModel.LogLevels);

            var result = new List<LogResult>();
            foreach (var log in logViews)
            {
                var formatted = log.MessageFormat;

                foreach (var property in log.LogProperties)
                {
                    formatted = formatted.Replace($"{{{property.Name}}}", property.Value);
                }
                result.Add(new LogResult
                {
                    ApplicationName = log.ApplicationName,
                    EnvironmentName = log.EnvironmentName,
                    Id = log.Id,
                    LogLevel = log.LogLevel,
                    LogProperties = log.LogProperties,
                    Message = formatted,
                    MessageFormat = log.MessageFormat,
                    RequestId = log.RequestId,
                    SourceName = log.SourceName,
                    Timestamp = log.Timestamp
                });
            }

            return result;
        }


        private async Task<IEnumerable<SelectionLogEnvironment>> GetLogEnvironments()
        {
            return (await _logReader.GetLogEnvironmentsAsync()).Select(le => 
            {
                if (le.Name == "Development")
                {
                    return new SelectionLogEnvironment { Id = le.Id, Name = le.Name, Selected = true };
                }
                return new SelectionLogEnvironment { Id = le.Id, Name = le.Name };
            });
        }

        private async Task<IEnumerable<SelectionLogApplication>> GetLogApplications()
        {
            return (await _logReader.GetLogApplicationsAsync()).Select(la => {
                if (la.ApplicationName == "TestApplication")
                {
                    return new SelectionLogApplication { Id = la.Id, Name = la.ApplicationName, Selected = true };
                }
                return new SelectionLogApplication { Id = la.Id, Name = la.ApplicationName };
            });
        }

        private IEnumerable<SelectionLogLevel> GetLogLevels()
        {
            return new List<SelectionLogLevel>
            {
                new SelectionLogLevel { Level = 0, Name = "Trace"},
                new SelectionLogLevel { Level = 1, Name = "Debug"},
                new SelectionLogLevel { Level = 2, Name = "Information"},
                new SelectionLogLevel { Level = 3, Name = "Warning", Selected = true},
                new SelectionLogLevel { Level = 4, Name = "Error", Selected = true},
                new SelectionLogLevel { Level = 5, Name = "Critical", Selected = true},

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
