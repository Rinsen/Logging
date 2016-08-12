using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Rinsen.Logger
{
    public class DatabaseLogReader : ILogReader
    {
        private readonly LogOptions _options;

        public DatabaseLogReader(LogOptions options)
        {
            _options = options;
        }

        public IEnumerable<LogItem> GetLatest(int count, LogLevel logLevel)
        {
            var results = new List<LogItem>(count);
            using(var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(string.Format("SELECT Id, SourceName, EnvironmentName, Timestamp, LogLevel, Message, ExceptionMessage, StackTrace FROM {0} WHERE LogLevel >= @LogLevel ORDER BY Id DESC", _options.LogItemsTableName), connection))
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

        private static LogItem MapLogItem(SqlDataReader reader)
        {
            return new LogItem
            {
                Id = (int)reader["Id"],
                EnvironmentName = (string)reader["EnvironmentName"],
                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), reader["LogLevel"].ToString()),
                Message = (string)reader["Message"],
                SourceName = (string)reader["SourceName"],
                ExceptionMessage = (string)reader["ExceptionMessage"],
                StackTrace = (string)reader["StackTrace"],
                Timestamp = (DateTimeOffset)reader["Timestamp"]
            };
        }

        public IEnumerable<LogItem> GetLatestFrom(int id, LogLevel logLevel)
        {
            var results = new List<LogItem>();
            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(string.Format("SELECT Id, SourceName, EnvironmentName, Timestamp, LogLevel, Message, ExceptionMessage, StackTrace FROM {0} WHERE LogLevel >= @LogLevel AND Id >= @Id ORDER BY Id DESC", _options.LogItemsTableName), connection))
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

        public IEnumerable<LogItem> GetMoreFrom(int id, int count, LogLevel logLevel)
        {
            var results = new List<LogItem>(count);
            using (var connection = new SqlConnection(_options.ConnectionString))
            using (var command = new SqlCommand(string.Format("SELECT Id, SourceName, EnvironmentName, Timestamp, LogLevel, Message, ExceptionMessage, StackTrace FROM {0} WHERE LogLevel >= @LogLevel AND Id >= @Id ORDER BY Id DESC", _options.LogItemsTableName), connection))
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
    }
}
