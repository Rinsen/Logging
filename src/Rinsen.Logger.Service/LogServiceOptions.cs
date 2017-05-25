namespace Rinsen.Logger.Service
{
    public class LogServiceOptions: LogOptions
    {
        public LogServiceOptions()
        {
            LogItemsTableName = "Logs";
        }

        public string ConnectionString { get; set; }

        public string LogItemsTableName { get; set; }

    }
}
