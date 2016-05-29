//using System.Collections.Generic;
//using Microsoft.AspNet.Hosting;
//using System.IO;

//namespace Rinsen.Logger
//{
//    public class FileLogWriter : ILogWriter
//    {
//        readonly IHostingEnvironment _env;

//        public FileLogWriter(IHostingEnvironment env)
//        {
//            _env = env;
//        }

//        public void WriteLogs(IEnumerable<LogItem> logItems)
//        {
//            using (var file = File.AppendText(_env.WebRootPath + "\\FileWriterLog1.txt"))
//            {
//                foreach (var logItem in logItems)
//                {
//                    file.WriteLine(string.Format("{0} - {1} - {2} - {3}", logItem.Timestamp, logItem.SourceName, logItem.LogLevel, logItem.Message));
//                }
//            }
//        }
//    }
//}
