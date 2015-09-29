using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Layout;
using NUnit.Framework;

namespace BrowserLog
{
    public class SseAppenderTest
    {
        [Test]
        public void StartLogger()
        {
            var layout = new PatternLayout("%-4timestamp [%thread] %-5level %logger %ndc - %message%newline");
            var appender = new SseAppender()
            {
                Host = "localhost",
                Port = 8765,
                Layout = layout
            };
            layout.ActivateOptions();
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);

            LogManager.GetLogger(GetType()).Info("Ca marche ?");
        }
    }
}
