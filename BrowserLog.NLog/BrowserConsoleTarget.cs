using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
            Name = "Console";
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
                    Host = FindLocalIp();
                }
                _channel = _channelFactory.Create(Host, Port, Buffer);
            }
        }

        // hack described here
        // http://stackoverflow.com/a/27376368
        //
        private static string FindLocalIp()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var message = base.Layout.Render(logEvent);

            if (Active)
            {
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
