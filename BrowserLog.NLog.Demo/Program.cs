using System;
using System.Threading;
using NLog;
using NLogConfig = NLog.Config;

namespace BrowserLog.NLog.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new NLogConfig.XmlLoggingConfiguration("NLog.config", true);

            var logger = LogManager.GetLogger("*", typeof(Program));
            logger.Info("Hello!");
            Thread.Sleep(1000);
            for (int i = 0; i < 100000; i++)
            {
                logger.Info("Hello this is a log from a server-side process!");
                Thread.Sleep(100);
                logger.Warn("Hello this is a warning from a server-side process!");
                logger.Debug("... and here is another log again ({0})", i);
                Thread.Sleep(200);
                try
                {
                    ThrowExceptionWithStackTrace(4);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "An error has occured, really?");
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
