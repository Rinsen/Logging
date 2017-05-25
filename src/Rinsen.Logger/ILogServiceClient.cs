using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public interface ILogServiceClient
    {
        Task<bool> ReportAsync(LogReport logReport);
    }
}
