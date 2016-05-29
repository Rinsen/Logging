using System.Collections.Generic;

namespace Rinsen.Logger
{
    public class DatabaseLogWriter : ILogWriter
    {
        private readonly LogOptions _options;

        public DatabaseLogWriter(LogOptions options)
        {
            _options = options;
        }

        public void WriteLogs(IEnumerable<LogItem> logItems)
        {
            
        }
    }
}
