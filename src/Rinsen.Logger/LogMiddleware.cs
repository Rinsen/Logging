using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMiddleware> _log;
        private readonly LogOptions _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly QueueLoggerProvider _queueLoggerProvider;
        private readonly LogHandler _logHandler;

        private static readonly object _sync = new object();
        private Task _logHandlerTask;

        private bool _initialized = false;
        

        public LogMiddleware(RequestDelegate next,
            ILogger<LogMiddleware> log,
            LogOptions options,
            ILoggerFactory loggerFactory,
            QueueLoggerProvider queueLoggerProvider,
            LogHandler logHandler)
        {
            _next = next;
            _log = log;
            _options = options;
            _loggerFactory = loggerFactory;
            _queueLoggerProvider = queueLoggerProvider;
            _logHandler = logHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!_initialized)
            {
                lock (_sync)
                {
                    if (!_initialized) // I should try to find a better solution to initialize log handler and inject provider..
                    {
                        _loggerFactory.AddProvider(_queueLoggerProvider);
                        _logHandlerTask = Task.Run(() => _logHandler.Start());
                        _initialized = true;
                    }
                }
            }

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
