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
                    To = DateTimeOffset.Now.AddHours(5)
                }
            };

            var selectionModel = GetSelectionModel();

            ApplySelectionModel(model, selectionModel);

            return View(model);
        }

        [HttpPost]
        public async Task<IEnumerable<LogResult>> GetLogs([FromBody]SearchModel searchModel)
        {
            UpdateSelectionModel(searchModel);

            var logViews = await _logReader.GetLogsAsync(searchModel.From, searchModel.To, searchModel.LogApplications, searchModel.LogEnvironments, searchModel.LogLevels);

            return logViews.Select(log => new LogResult(log)).ToArray();
        }

        private async Task<IEnumerable<SelectionLogEnvironment>> GetLogEnvironments()
        {
            return (await _logReader.GetLogEnvironmentsAsync()).Select(le => 
            {
                return new SelectionLogEnvironment { Id = le.Id, Name = le.Name };
            }).ToArray();
        }

        private async Task<IEnumerable<SelectionLogApplication>> GetLogApplications()
        {
            return (await _logReader.GetLogApplicationsAsync()).Select(la => 
            {
                return new SelectionLogApplication { Id = la.Id, Name = la.ApplicationName };
            }).ToArray();
        }

        private IEnumerable<SelectionLogLevel> GetLogLevels()
        {
            return new List<SelectionLogLevel>
            {
                new SelectionLogLevel { Level = 0, Name = "Trace"},
                new SelectionLogLevel { Level = 1, Name = "Debug"},
                new SelectionLogLevel { Level = 2, Name = "Information"},
                new SelectionLogLevel { Level = 3, Name = "Warning" },
                new SelectionLogLevel { Level = 4, Name = "Error" },
                new SelectionLogLevel { Level = 5, Name = "Critical" },
            };
        }

        private void UpdateSelectionModel(SearchModel searchModel)
        {
            
        }

        private SelectionModel GetSelectionModel()
        {
            return new SelectionModel
            {
                LogApplications = new List<int> { 1, 2, 3, 4 },
                LogEnvironments = new List<int> { 1, 2, 3 },
                LogLevels = new List<int> { 2, 3, 4, 5 }
            };
        }

        private void ApplySelectionModel(LoggerModel model, SelectionModel selectionModel)
        {
            foreach (var logApplication in model.SelectionOptions.LogApplications)
            {
                if (selectionModel.LogApplications.Contains(logApplication.Id))
                {
                    logApplication.Selected = true;
                }
            }

            foreach (var logEnvironments in model.SelectionOptions.LogEnvironments)
            {
                if (selectionModel.LogEnvironments.Contains(logEnvironments.Id))
                {
                    logEnvironments.Selected = true;
                }
            }

            foreach (var logLevel in model.SelectionOptions.LogLevels)
            {
                if (selectionModel.LogLevels.Contains(logLevel.Level))
                {
                    logLevel.Selected = true;
                }
            }
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
