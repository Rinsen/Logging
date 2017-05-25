using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public interface ILogWriter
    {
        Task WriteLogsAsync(IEnumerable<Log> logItems);
        Task<LogEnvironment> CreateLogEnvironmentAsync(string environmentName);
    }
}
