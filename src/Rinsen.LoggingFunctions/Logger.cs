using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Rinsen.Logger;
using Rinsen.Logger.Service;

namespace Rinsen.LoggingFunctions
{
    public static class Logger
    {
        public static async Task<IActionResult> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<LogReport>(jsonContent);

            var logOptions = new LogServiceOptions { ConnectionString = "" };
            var logHandler = new LogHandler(new DatabaseLogReader(logOptions), new DatabaseLogWriter(logOptions));

            // Fetching the name from the path parameter in the request URL
            return new OkObjectResult("Hello ");
        }
    }
}
