using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rinsen.Logger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILogReader _logReader;

        public HomeController(ILogger<HomeController> logger, ILogReader logReader)
        {
            _logger = logger;
            _logReader = logReader;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            _logger.LogDebug("Display index");

            return View();
        }

        // GET: /<controller>/
        public IActionResult Error()
        {
            throw new NotImplementedException("Not done", new Exception("My inner exception"));
        }

        public IActionResult DisplayLogs()
        {
            var logs = _logReader.GetLatest(30, LogLevel.Trace).OrderByDescending(m => m.Id);

            _logger.LogError("This is a error");

            Task.Delay(10000).Wait();

            var latestLogs = _logReader.GetLatestFrom(logs.First().Id, LogLevel.Trace);

            var moreLogs = _logReader.GetMoreFrom(logs.Last().Id, 10, LogLevel.Trace);

            return View();
        }
    }
}
