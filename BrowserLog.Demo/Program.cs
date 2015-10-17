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
            logger.Info("Hello!");
            Thread.Sleep(10000);
            for (int i = 0; i < 100000; i++)
            {
                logger.Info("Hello this is a log from a server-side process!");
                Thread.Sleep(1000);
                logger.Warn("Hello this is a warning from a server-side process!");
                logger.DebugFormat("... and here is another log again ({0})", i);
                Thread.Sleep(2000);
                try
                {
                    ThrowExceptionWithStackTrace(4);
                }
                catch (Exception ex)
                {
                    logger.Error("An error has occured, really?", ex);    
                }
                
                Thread.Sleep(1000);            
            }
        }

        static void ThrowExceptionWithStackTrace(int depth)
        {
            if (depth == 0)
            {
                throw new Exception("A fake exception to show an example");
            }
            ThrowExceptionWithStackTrace(--depth);
        }
    }
}
