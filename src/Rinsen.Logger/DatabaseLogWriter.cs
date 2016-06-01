using System.Collections.Generic;
using System.Data.SqlClient;

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
            string insertSql = string.Format(@"
                                INSERT INTO {0} (
                                    SourceName,
                                    EnvironmentName,
                                    LogLevel,
                                    Message,
                                    TimeStamp,
                                    StackTrace) 
                                VALUES (
                                    @SourceName,
                                    @EnvironmentName,
                                    @LogLevel,
                                    @Message,
                                    @Timestamp,
                                    @StackTrace); 
                                SELECT 
                                    CAST(SCOPE_IDENTITY() as int)"
                                , _options.LogItemsTableName);

            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                connection.Open();
                foreach (var item in logItems)
                {
                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@SourceName", item.SourceName));
                        command.Parameters.Add(new SqlParameter("@EnvironmentName", item.EnvironmentName));
                        command.Parameters.Add(new SqlParameter("@LogLevel", item.LogLevel));
                        command.Parameters.Add(new SqlParameter("@Message", item.Message));
                        command.Parameters.Add(new SqlParameter("@Timestamp", item.Timestamp));
                        command.Parameters.Add(new SqlParameter("@StackTrace", item.StackTrace));

                        item.Id = (int)command.ExecuteScalar();
                    }
                }
            }
        }
    }
}
