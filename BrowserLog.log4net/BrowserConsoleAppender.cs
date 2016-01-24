using System;
using System.Threading;
using BrowserLog.Common;
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
        public int Buffer { get; set; }

        public BrowserConsoleAppender()
        {
            _channelFactory = new ChannelFactory();
            Active = true;
            Port = 8765;
            Buffer = 1;
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
                if (Host == null)
                {
                    Host = DefaultHostFinder.FindLocalIp();
                }
                _channel = _channelFactory.Create(Host, Port, Buffer);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var message = base.RenderLoggingEvent(loggingEvent);
            var sse = new ServerSentEvent(loggingEvent.Level.DisplayName, message);
            _channel.Send(sse, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        }

        protected override bool PreAppendCheck()
        {
            if (!Active)
            {
                return false;
            }
            return base.PreAppendCheck();
        }

        protected override void OnClose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }
        }
    }
}