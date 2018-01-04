using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class LogServiceClient : ILogServiceClient
    {
        private LogOptions _options;

        public LogServiceClient(LogOptions options)
        {
            _options = options;
        }

        public async Task<bool> ReportAsync(LogReport logReport)
        {
            using (var httpClient = new HttpClient())
            {
#if DEBUG
                httpClient.Timeout = TimeSpan.FromMinutes(10);
#endif
                var serializedLogs = JsonConvert.SerializeObject(logReport);
                var stringContent = new StringContent(serializedLogs, Encoding.UTF8, "application/json");
                using (var result = await httpClient.PostAsync($"{_options.LogServiceUri}Logger/ReportAsync", stringContent))
                {
                    result.EnsureSuccessStatusCode();
                    var result2 = await result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(result2);
                }
            }
        }
    }
}
