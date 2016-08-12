using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public interface ILogReader
    {
        IEnumerable<LogItem> GetLatest(int count, LogLevel logLevel);
        IEnumerable<LogItem> GetLatestFrom(int id, LogLevel logLevel);
        IEnumerable<LogItem> GetMoreFrom(int id, int count, LogLevel logLevel);
    }
}
