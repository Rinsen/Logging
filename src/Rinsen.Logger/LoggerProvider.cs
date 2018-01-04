using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class QueueLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ILogQueue _logQueue;
        private readonly ILogServiceClient _logServiceClient;
        private readonly LogOptions _options;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _logHandlerTask;
        private static bool _initialized = false;
        private static readonly object _sync = new object();

        public QueueLoggerProvider(ILogQueue logQueue, ILogServiceClient logServiceClient, LogOptions options)
        {
            _logQueue = logQueue;
            _logServiceClient = logServiceClient;
            _options = options;
            _filter = (category, logLevel) => logLevel >= options.MinLevel && category.StartsWith("");
            Initializer();
        }

        public ILogger CreateLogger(string name)
        {
            return new Logger(name, _filter, _logQueue);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            if (_initialized)
            {
                StopProcessing();
            }
        }

        private void Initializer()
        {
            lock (_sync)
            {
                if (_initialized)
                    return;

                _cancellationTokenSource = new CancellationTokenSource();

                _logHandlerTask = Task.Factory.StartNew(ProcessLogQueue, null, TaskCreationOptions.LongRunning);

                _initialized = true;
            }
        }

        private void StopProcessing()
        {
            _cancellationTokenSource.Cancel();

            try
            {
                _logHandlerTask.Wait(_options.TimeToSleepBetweenBatches);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        internal async Task ProcessLogQueue(object state)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var logs = _logQueue.GetReportedLogs();
                    if (logs.Any())
                    {
                        var result = await _logServiceClient.ReportAsync(new LogReport { ApplicationKey = _options.ApplicationLogKey, LogItems = logs });
                    }
                }
                catch (Exception e)
                {
                }

                await Task.Delay(_options.TimeToSleepBetweenBatches, _cancellationTokenSource.Token);
            }
        }
    }
}
