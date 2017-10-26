using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace Rinsen.Logger.Service
{
    public class DatabaseLogReader : ILogReader
    {
        private readonly LogServiceOptions _options;

        public DatabaseLogReader(LogServiceOptions options)
        {
            _options = options;
        }

        public IEnumerable<Log> GetLatest(int count, LogLevel logLevel)
        {
            var results = new List<Log>(count);
            using(var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(string.Format(@"SELECT Id,
                                                                    Application,
                                                                    SourceName, 
                                                                    EnvironmentName, 
                                                                    RequestId, 
                                                                    LogLevel, 
                                                                    MessageFormat, 
                                                                    SerializedLogProperties, 
                                                                    ExceptionMessage, 
                                                                    StackTrace, 
                                                                    Timestamp 
                                                                        FROM {0} 
                                                                        WHERE 
                                                                            LogLevel >= @LogLevel 
                                                                        ORDER BY Id DESC"
                                                                            , _options.LogItemsTableName), connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("@LogLevel", logLevel));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        var number = 0;
                        while (reader.Read())
                        {
                            results.Add(MapLogItem(reader));

                            number++;

                            if (number >= count)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return results;
        }

        private static Log MapLogItem(SqlDataReader reader)
        {
            return new Log
            {
                Id = (int)reader["Id"],
                //Application = (string)reader["Application"],
                //EnvironmentName = (string)reader["EnvironmentName"],
                RequestId = (string)reader["RequestId"],
                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), reader["LogLevel"].ToString()),
                MessageFormat = (string)reader["MessageFormat"],
                //SerializedLogProperties = (string)reader["SerializedLogProperties"],
                SourceName = (string)reader["SourceName"],
                //ExceptionMessage = (string)reader["ExceptionMessage"],
                //StackTrace = (string)reader["StackTrace"],
                Timestamp = (DateTimeOffset)reader["Timestamp"]
            };
        }

        public IEnumerable<Log> GetLatestFrom(int id, LogLevel logLevel)
        {
            var results = new List<Log>();
            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(string.Format(@"SELECT Id,
                                                                    Application,
                                                                    SourceName, 
                                                                    EnvironmentName, 
                                                                    RequestId, 
                                                                    LogLevel, 
                                                                    MessageFormat, 
                                                                    SerializedLogProperties, 
                                                                    ExceptionMessage, 
                                                                    StackTrace, 
                                                                    Timestamp 
                                                                FROM {0} 
                                                                WHERE LogLevel >= @LogLevel 
                                                                        AND Id >= @Id ORDER BY Id DESC", _options.LogItemsTableName), connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("@LogLevel", logLevel));
                command.Parameters.Add(new SqlParameter("@Id", id));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            results.Add(MapLogItem(reader));
                        }
                    }
                }
            }

            return results;
        }

        public IEnumerable<Log> GetMoreFrom(int id, int count, LogLevel logLevel)
        {
            var results = new List<Log>(count);
            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(string.Format(@"SELECT Id,
                                                                    Application,
                                                                    SourceName, 
                                                                    EnvironmentName, 
                                                                    RequestId, 
                                                                    LogLevel, 
                                                                    MessageFormat, 
                                                                    SerializedLogProperties, 
                                                                    ExceptionMessage, 
                                                                    StackTrace, 
                                                                    Timestamp 
                                                                FROM {0} 
                                                                WHERE LogLevel >= @LogLevel AND Id >= @Id ORDER BY Id DESC", _options.LogItemsTableName), connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("@LogLevel", logLevel));
                command.Parameters.Add(new SqlParameter("@Id", id));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        var number = 0;
                        while (reader.Read())
                        {
                            results.Add(MapLogItem(reader));

                            number++;

                            if (number >= count)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return results;
        }

        public async Task<List<LogEnvironment>> GetLogEnvironmentsAsync()
        {
            var results = new List<LogEnvironment>();

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, Name FROM LogEnvironments", connection))
            {
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            results.Add(new LogEnvironment
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"]
                            });
                        }
                    }
                }
            }

            return results;
        }

        public async Task<List<LogApplication>> GetLogApplicationsAsync()
        {
            var logApplications = new List<LogApplication>();

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, ApplicationKey, ApplicationName FROM LogApplications", connection))
            {
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            logApplications.Add(new LogApplication
                            {
                                Id = (int)reader["Id"],
                                ApplicationKey = (string)reader["ApplicationKey"],
                                ApplicationName = (string)reader["ApplicationName"]
                            });
                        }
                    }
                }
            }

            return logApplications;
        }

        public async Task<LogApplication> GetLogApplicationAsync(string applicationKey)
        {
            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("SELECT Id, ApplicationKey, ApplicationName FROM LogApplications WHERE ApplicationKey = @ApplicationKey", connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("@ApplicationKey", applicationKey));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return new LogApplication
                            {
                                Id = (int)reader["Id"],
                                ApplicationKey = (string)reader["ApplicationKey"],
                                ApplicationName = (string)reader["ApplicationName"]
                            };
                        }
                    }
                }
            }

            throw new Exception($"Application {applicationKey} not found");
        }

        public async Task<IEnumerable<LogView>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> logLevels, int take = 200)
        {
            var logs = new List<LogView>();
            var taken = 0;

            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand("", connection))
            {
                CreateCommandSqlAndParameters(command, from, to, logApplications, logEnvironments, logLevels);

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read() && taken < take)
                        {
                            logs.Add(new LogView
                            {
                                Id = (int)reader["Id"],
                                ApplicationName = (string)reader["ApplicationName"],
                                EnvironmentName = (string)reader["Name"],
                                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), reader["LogLevel"].ToString()),
                                LogProperties = JsonConvert.DeserializeObject<IEnumerable<LogProperty>>((string)reader["LogProperties"]),
                                MessageFormat = (string)reader["MessageFormat"],
                                RequestId = (string)reader["RequestId"],
                                SourceName = (string)reader["SourceName"],
                                Timestamp = (DateTimeOffset)reader["Timestamp"]
                            });
                            taken++;
                        }
                    }
                }
            }

            return logs;
        }

        private void CreateCommandSqlAndParameters(SqlCommand command, DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> logLevels)
        {
            command.Parameters.Add(new SqlParameter("@from", from));
            command.Parameters.Add(new SqlParameter("@to", to));

            var sql = new StringBuilder(@"SELECT Logs.Id,
                                                        Logs.LogLevel,
                                                        Logs.LogProperties,
                                                        Logs.MessageFormat,
                                                        Logs.RequestId,
                                                        Logs.SourceName,
                                                        Logs.Timestamp,
                                                        LogApp.ApplicationName,
                                                        LogEnv.Name
	                                                FROM Logs
                                                        JOIN LogApplications LogApp ON Logs.ApplicationId = LogApp.Id
                                                        JOIN LogEnvironments LogEnv ON Logs.EnvironmentId = LogEnv.Id
                                                    WHERE Logs.Timestamp > @from 
                                                        AND Logs.Timestamp < @to 
                                                        AND Logs.ApplicationId IN (");

            var count = 0;
            foreach (var logApplication in logApplications)
            {
                command.Parameters.Add(new SqlParameter($"@la{count}", logApplication));
                if (count == 0)
                {
                    sql.Append($"@la{count}");
                }
                else
                {
                    sql.Append($", @la{count}");
                }
                count++;
            }
            sql.Append(") AND Logs.EnvironmentId IN (");

            count = 0;
            foreach (var logEnvironment in logEnvironments)
            {
                command.Parameters.Add(new SqlParameter($"@le{count}", logEnvironment));
                if (count == 0)
                {
                    sql.Append($"@le{count}");
                }
                else
                {
                    sql.Append($", @le{count}");
                }
                count++;
            }
            sql.Append(") AND Logs.LogLevel IN (");

            count = 0;
            foreach (var logLevel in logLevels)
            {
                command.Parameters.Add(new SqlParameter($"@level{count}", logLevel));
                if (count == 0)
                {
                    sql.Append($"@level{count}");
                }
                else
                {
                    sql.Append($", @level{count}");
                }
                count++;
            }
            sql.Append(")");

            command.CommandText = sql.ToString();

        }
    }
}
