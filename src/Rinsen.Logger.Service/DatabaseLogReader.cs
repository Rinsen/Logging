using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Threading.Tasks;

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

        public Task<IEnumerable<LogApplication>> GetLogApplicationsAsync()
        {
            throw new NotImplementedException();
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
    }
}
