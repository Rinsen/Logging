using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rinsen.Logger
{
    public interface IRinsenLoggerInitializer
    {
        void Run(IFilterLoggerSettings filterLoggerSettings = null);
    }
}
