using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logging.Models
{
    public class SearchModel
    {
        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }

        public IEnumerable<int> LogLevels { get; set; }

        public IEnumerable<int> LogEnvironments { get; set; }

        public IEnumerable<int> LogApplications { get; set; }

    }
}
