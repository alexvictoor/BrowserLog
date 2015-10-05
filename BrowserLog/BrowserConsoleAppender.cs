using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrowserLog.TinyServer;
using log4net.Appender;
using log4net.Core;

namespace BrowserLog
{
    public class BrowserConsoleAppender : AppenderSkeleton
    {

        private readonly ChannelFactory _channelFactory;
        private IEventChannel _channel;

        public bool Active { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }

        public BrowserConsoleAppender()
        {
            _channelFactory = new ChannelFactory();
            Active = true;
            Host = "127.0.0.1";
        }

        // for testing
        public BrowserConsoleAppender(ChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
        }

        public override void ActivateOptions()
        {
            if (Active)
            {
                _channel = _channelFactory.Create(Host, Port);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var message = base.RenderLoggingEvent(loggingEvent);
            var sse = new ServerSentEvent(loggingEvent.Level.DisplayName, message);
            _channel.Send(sse);
        }
    }
}
