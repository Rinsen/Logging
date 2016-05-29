using System;
using Microsoft.Extensions.Logging;

namespace Rinsen.Logger
{
    public class LogOptions
    {
        public LogOptions()
        {
            QueueMazSize = 2000;
            MaxBatchSize = 200;
            TimeToSleepBetweenBatches = new TimeSpan(0, 0, 10);
            MinLevel = LogLevel.Information;
            LogItemsTableName = "LogItems";
        }

        public string ConnectionString { get; set; }

        public int QueueMazSize { get; set; }

        public int MaxBatchSize { get; set; }

        public TimeSpan TimeToSleepBetweenBatches { get; set; }

        public LogLevel MinLevel { get; set; }

        public string LogItemsTableName { get; set; }

        public string EnvironmentName { get; set; }
    }
}
