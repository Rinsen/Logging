using Rinsen.DatabaseInstaller;
using Rinsen.DatabaseInstaller.SqlTypes;
using System.Collections.Generic;

namespace Rinsen.Logger.Service.Installation
{
    public class CreateLogTable : DatabaseVersion
    {
        public CreateLogTable()
        : base(1)
        { }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var logEnvironmentTable = dbChangeList.AddNewTable<LogEnvironment>();
            logEnvironmentTable.AddAutoIncrementColumn(m => m.Id);
            logEnvironmentTable.AddColumn(m => m.Name, 100).Unique().NotNull();

            var logsTable = dbChangeList.AddNewTable<Log>();
            logsTable.AddAutoIncrementColumn(m => m.Id);
            logsTable.AddColumn(m => m.ApplicationId).NotNull().ForeignKey("ExternalApplications", "Id");
            logsTable.AddColumn(m => m.EnvironmentId).NotNull().ForeignKey<LogEnvironment>(m => m.Id);
            logsTable.AddColumn(m => m.RequestId, 100).NotNull();
            logsTable.AddIntColumn("LogLevel").NotNull();
            logsTable.AddColumn(m => m.MessageFormat).NotNull();
            logsTable.AddColumn("LogProperties", new NVarChar()).NotNull();
            logsTable.AddColumn(m => m.SourceName).NotNull();
            logsTable.AddColumn(m => m.Timestamp).NotNull();
        }
    }
}


