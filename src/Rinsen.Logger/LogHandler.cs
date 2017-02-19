using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class LogHandler
    {
        public List<ILogWriter> LogWriters { get; private set; }

        readonly LogOptions _options;
        readonly ILogQueue _logQueue;
        
        public LogHandler(LogOptions options, ILogQueue logQueue, ILogWriter logWriter)
        {
            _options = options;
            _logQueue = logQueue;
            LogWriters = new List<ILogWriter>
            {
                logWriter
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Potential Code Quality Issues", "RECS0022:A catch clause that catches System.Exception and has an empty body", Justification = "Failing ILogWriter should only try to be logged in other log writers and not throw any exceptions")]
        internal void Start()
        {
            while (true)
            {
                var logs = _logQueue.GetLogs();

                if (logs.Any())
                {
                    var logWriterExceptions = new List<Exception>();

                    foreach (var logWriter in LogWriters)
                    {
                        try
                        {
                            logWriter.WriteLogs(logs);
                        }
                        catch (Exception e)
                        {
                            logWriterExceptions.Add(e);
                        }
                    }

                    if (logWriterExceptions.Any())
                    {
                        foreach (var logWriter in LogWriters)
                        {
                            try
                            {
                                logWriter.WriteLogs(logs);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                Task.Delay(_options.TimeToSleepBetweenBatches).Wait();
            }
        }
    }
}
