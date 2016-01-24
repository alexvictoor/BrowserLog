using System;
using System.Configuration;
using System.Threading;
using BrowserLog.Common;
using Serilog;

namespace BrowserLog.Serilog.Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            //Configuration by AppSettings
            var logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .MinimumLevel.Debug()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("MyMetaProperty", "Oh! the beautiful value!")
                .WriteTo.ColoredConsole()
                .CreateLogger();

            ////Configuration by code
            //var logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .Enrich.WithThreadId()
            //    .Enrich.WithProperty("MyMetaProperty", "Oh! the beautiful value!")
            //    .WriteTo.ColoredConsole()
            //    .WriteTo.BrowserConsole(port: 9999, buffer: 50)
            //    .CreateLogger();

            OpenBrowserToBrowserLogUrl();

            logger.Information("Hello!");
            Thread.Sleep(1000);
            for (int i = 0; i < 100000; i++)
            {
                logger.Information("Hello this is a log from a server-side process!");
                Thread.Sleep(100);
                logger.Warning("Hello this is a warning from a server-side process!");
                logger.Debug("... and here is another log again ({IndexLoop})", i);
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

        private static void OpenBrowserToBrowserLogUrl()
        {
            try
            {
                var port = ConfigurationManager.AppSettings["serilog:write-to:BrowserConsole.port"];
                if (port == null)
                    return;
                var url = "http://" + DefaultHostFinder.FindLocalIp() + ":" + port;
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
