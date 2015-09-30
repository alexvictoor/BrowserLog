using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace BrowserLog.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.xml"));

            var logger = LogManager.GetLogger(typeof(Program));
            logger.Info("Hello this is a log from a server-side process!");
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1000);
                logger.DebugFormat("... and here is another one again ({0})", i);
            }
        }
    }
}
