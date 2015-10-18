using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                    Host = FindLocalIp();
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

        protected override void OnClose()
        {
            if (_channel != null)
            {
                _channel.Dispose();    
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
    }
}
