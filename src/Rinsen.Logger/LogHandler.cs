using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class LogHandler
    {
        readonly LogOptions _options;
        readonly ILogQueue _logQueue;
        readonly ILogServiceClient _logServiceClient;
        
        public LogHandler(LogOptions options, ILogQueue logQueue, ILogServiceClient logServiceClient)
        {
            _options = options;
            _logQueue = logQueue;
            _logServiceClient = logServiceClient;
        }

        internal async Task SendAsync()
        {
            var logs = _logQueue.GetReportedLogs();
            if (logs.Any())
            {
                var result = await _logServiceClient.ReportAsync(new LogReport { ApplicationKey = _options.ApplicationLogKey, LogItems = logs });
            }
        }
    }
}
