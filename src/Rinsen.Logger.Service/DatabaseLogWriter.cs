using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class DatabaseLogWriter : ILogWriter
    {
        private readonly LogServiceOptions _options;

        public DatabaseLogWriter(LogServiceOptions options)
        {
            _options = options;
        }

        public async Task<LogEnvironment> CreateLogEnvironmentAsync(string environmentName)
        {
            string insertSql = @"INSERT INTO LogEnvironments (
                                    Name) 
                                 VALUES (
                                    @Name);
                                 SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";
                                

            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Name", environmentName));
                    return new LogEnvironment
                    {
                        Id = (int)await command.ExecuteScalarAsync(),
                        Name = environmentName
                    };
                }
            }
        }

        public async Task WriteLogsAsync(IEnumerable<Log> logs)
        {
            string insertSql = @"
                                INSERT INTO Logs (
                                    ApplicationId,
                                    SourceName, 
                                    EnvironmentId, 
                                    RequestId, 
                                    LogLevel, 
                                    MessageFormat, 
                                    LogProperties, 
                                    Timestamp) 
                                VALUES (
                                    @ApplicationId,
                                    @SourceName, 
                                    @EnvironmentId, 
                                    @RequestId, 
                                    @LogLevel, 
                                    @MessageFormat, 
                                    @LogProperties,
                                    @Timestamp);
                                SELECT 
                                    CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                connection.Open();
                foreach (var item in logs)
                {
                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@ApplicationId", item.ApplicationId));
                        command.Parameters.Add(new SqlParameter("@SourceName", item.SourceName));
                        command.Parameters.Add(new SqlParameter("@EnvironmentId", item.EnvironmentId));
                        command.Parameters.Add(new SqlParameter("@RequestId", item.RequestId));
                        command.Parameters.Add(new SqlParameter("@LogLevel", item.LogLevel));
                        command.Parameters.Add(new SqlParameter("@MessageFormat", item.MessageFormat));
                        var properties = JsonConvert.SerializeObject(item.LogProperties);
                        command.Parameters.Add(new SqlParameter("@LogProperties", properties));
                        command.Parameters.Add(new SqlParameter("@Timestamp", item.Timestamp));

                        item.Id = (int) await command.ExecuteScalarAsync();
                    }
                }
            }
        }
    }
}
