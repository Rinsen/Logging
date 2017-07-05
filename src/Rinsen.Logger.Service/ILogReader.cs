using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Rinsen.Logger.Service
{
    public interface ILogReader
    {
        IEnumerable<Log> GetLatest(int count, LogLevel logLevel);
        IEnumerable<Log> GetLatestFrom(int id, LogLevel logLevel);
        IEnumerable<Log> GetMoreFrom(int id, int count, LogLevel logLevel);
        Task<List<LogApplication>> GetLogApplicationsAsync();
        Task<List<LogEnvironment>> GetLogEnvironmentsAsync();
        Task<LogApplication> GetLogApplicationAsync(string applicationKey);
        Task<IEnumerable<LogView>> GetLogs(DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> logLevels);
    }
}
