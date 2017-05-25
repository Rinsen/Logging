using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LoggerSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var id = 1;
            _logger.LogDebug("Display index {id}", id);

            return View();
        }

        // GET: /<controller>/
        public IActionResult Error()
        {
            throw new NotImplementedException("Not done", new Exception("My inner exception"));
        }

    }
}
