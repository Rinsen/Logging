using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Rinsen.Logger.Service
{
    public interface ILogReader
    {
        Task<List<LogApplication>> GetLogApplicationsAsync();
        Task<List<LogEnvironment>> GetLogEnvironmentsAsync();
        Task<LogApplication> GetLogApplicationAsync(string applicationKey);
        Task<IEnumerable<LogView>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> logLevels, int take = 200);
    }
}
