using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BrowserLog.Commom;
using BrowserLog.TinyServer;
using NLog;
using NLog.Targets;

namespace BrowserLog.NLog
{
    [Target("BrowserConsole")]
    public class BrowserConsoleTarget : TargetWithLayout
    {
        private readonly ChannelFactory _channelFactory;
        private IEventChannel _channel;

        public bool Active { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public int Buffer { get; set; }

        public BrowserConsoleTarget()
        {
            _channelFactory = new ChannelFactory();
            Active = true;
            Port = 8765;
            Buffer = 1;
            Name = "BrowserConsole";
        }

        // for testing
        public BrowserConsoleTarget(ChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (Active)
            {
                if (Host == null)
                {
                    Host = DefaultHostFinder.FindLocalIp();
                }
                _channel = _channelFactory.Create(Host, Port, Buffer);
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (Active)
            {
                var message = base.Layout.Render(logEvent);
                var sse = new ServerSentEvent(logEvent.Level.Name.ToUpperInvariant(), message);
                _channel.Send(sse, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
            }
        }

        protected override void CloseTarget()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }
            base.CloseTarget();
        }
    }
}
