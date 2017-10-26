using Microsoft.AspNetCore.Mvc;
using Rinsen.Logger;
using System.Threading.Tasks;
using Rinsen.Logger.Service;
using System.Linq;
using System.Collections.Generic;
using Rinsen.Logging.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Rinsen.Logging.Controllers
{
    public class LoggerController : Controller, ILogServiceClient
    {
        private readonly ILogWriter _logWriter;
        private readonly ILogReader _logReader;
        private readonly LogHandler _logHandler;

        public LoggerController(ILogReader logReader, ILogWriter logWriter, LogHandler logHandler)
        {
            _logReader = logReader;
            _logWriter = logWriter;
            _logHandler = logHandler;
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
            var logViews = await _logReader.GetLogsAsync(searchModel.From, searchModel.To, searchModel.LogApplications, searchModel.LogEnvironments, searchModel.LogLevels);
            
            return logViews.Select(log => new LogResult(log));
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
        [AllowAnonymous]
        public Task<bool> ReportAsync([FromBody]LogReport logReport)
        {
            return _logHandler.CreateLogs(logReport);
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
