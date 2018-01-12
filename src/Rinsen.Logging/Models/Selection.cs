using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logging.Models
{
    public class SelectionModel
    {
        public IEnumerable<int> LogLevels { get; set; }

        public IEnumerable<int> LogEnvironments { get; set; }

        public IEnumerable<int> LogApplications { get; set; }
    }
}
