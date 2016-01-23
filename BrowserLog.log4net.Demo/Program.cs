using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;

namespace BrowserLog.log4net.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.xml"));

            var logger = LogManager.GetLogger(typeof(Program));

            OpenBrowserToBrowserLogUrl(logger);

            logger.Info("Hello!");
            Thread.Sleep(1000);
            for (int i = 0; i < 100000; i++)
            {
                logger.Info("Hello this is a log from a server-side process!");
                Thread.Sleep(100);
                logger.Warn("Hello this is a warning from a server-side process!");
                logger.DebugFormat("... and here is another log again ({0})", i);
                Thread.Sleep(200);
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

        private static void OpenBrowserToBrowserLogUrl(ILog logger)
        {
            try
            {
                var appender = (BrowserConsoleAppender) logger.Logger.Repository.GetAppenders().First(a =>a.Name == "WEB");
                var url = "http://" + appender.Host + ":" + appender.Port;
                Console.WriteLine("Opening BrowserLog url '" + url + "'... (Display the debugging console to see them.)");
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception)
            {
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
