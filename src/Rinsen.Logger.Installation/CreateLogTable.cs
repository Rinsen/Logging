using Rinsen.DatabaseInstaller;
using Rinsen.DatabaseInstaller.Sql;
using Rinsen.DatabaseInstaller.Sql.Generic;
using System.Collections.Generic;

namespace Rinsen.Logger.Installation
{
    public class CreateLogTable : DatabaseVersion
    {
        public CreateLogTable()
        : base(1)
        { }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var table = dbChangeList.AddNewTable<LogItem>("LogItems");
            table.AddAutoIncrementColumn(m => m.Id);
            table.AddColumn(m => m.SourceName, 1000).NotNull();
            table.AddColumn(m => m.EnvironmentName, 100).NotNull();
            table.AddColumn(m => m.Timestamp).NotNull();
            table.AddIntColumn("LogLevel").NotNull();
            table.AddColumn(m => m.ExceptionMessage).NotNull();
            table.AddColumn(m => m.Message).NotNull();
            table.AddColumn(m => m.StackTrace).NotNull();
        }
    }
}


