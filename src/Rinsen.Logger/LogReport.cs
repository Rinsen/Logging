using System.Collections.Generic;

namespace Rinsen.Logger
{
    public class LogReport
    {
        public string ApplicationKey { get; set; }

        public IEnumerable<LogItem> LogItems { get; set; }

    }
}
