using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rinsen.Logger
{
    public class RinsenLoggerInitializer : IRinsenLoggerInitializer
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly LogHandler _logHandler;
        private readonly LogOptions _options;
        private readonly QueueLoggerProvider _queueLoggerProvider;

        private bool _initialized = false;
        private readonly object _sync = new object();
        private Task _logHandlerTask;

        public RinsenLoggerInitializer(LogOptions options,
            ILoggerFactory loggerFactory,
            QueueLoggerProvider queueLoggerProvider,
            LogHandler logHandler)
        {
            _options = options;
            _loggerFactory = loggerFactory;
            _queueLoggerProvider = queueLoggerProvider;
            _logHandler = logHandler;
        }

        public void Run()
        {
            lock (_sync)
            {
                if (_initialized)
                    return;

                _loggerFactory.AddProvider(_queueLoggerProvider);
                _logHandlerTask = Task.Run(() => _logHandler.Start());
                _initialized = true;
            }

        }
    }
}
