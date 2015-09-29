using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace BrowserLog
{
    public class SseAppender : AppenderSkeleton
    {
       
        public bool Active { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }

        public SseAppender()
        {
            Active = true;
            Host = "+";
        }


        public override void ActivateOptions()
        {
            if (Active) { }
        }


        protected override void Append(LoggingEvent loggingEvent)
        {
            throw new NotImplementedException();
        }
    }
}
