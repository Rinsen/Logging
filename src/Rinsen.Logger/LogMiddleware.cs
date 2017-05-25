using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMiddleware> _log;

        public LogMiddleware(RequestDelegate next,
            ILogger<LogMiddleware> log
            )
        {
            _next = next;
            _log = log;
            
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _log.LogError(0, e, "Unhandled Exception");
                throw;
            }
        }
    }
}
