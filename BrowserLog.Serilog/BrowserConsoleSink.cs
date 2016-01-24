using System;
using System.IO;
using System.Linq;
using System.Threading;
using BrowserLog.Common;
using BrowserLog.TinyServer;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace BrowserLog.Serilog
{
    public class BrowserConsoleSink : ILogEventSink, IDisposable
    {
        private ChannelFactory _channelFactory;
        private IEventChannel _channel;
        public MessageTemplateTextFormatter Formatter { get; set; }
        public bool Active { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int Buffer { get; set; }
        public bool LogProperties { get; set; }

        public BrowserConsoleSink(bool active, int port, int buffer, MessageTemplateTextFormatter formatter, bool logProperties)
        {
            Active = active;
            Port = port;
            Buffer = buffer;
            LogProperties = logProperties;
            Formatter = formatter;

            CreateChannel();
        }

        private void CreateChannel(ChannelFactory channelFactory = null)
        {
            if (Active)
            {
                _channelFactory = channelFactory ?? new ChannelFactory();
                if (Host == null)
                {
                    Host = DefaultHostFinder.FindLocalIp();
                }
                _channel = _channelFactory.Create(Host, Port, Buffer);
            }
        }

        // for testing
        internal BrowserConsoleSink(bool active, int port, int buffer, MessageTemplateTextFormatter formatter, bool logProperties, ChannelFactory channelFactory)
        {
            Active = active;
            Port = port;
            Buffer = buffer;
            LogProperties = logProperties;
            Formatter = formatter;

            CreateChannel(channelFactory);
        }

        public void Emit(LogEvent logEvent)
        {
            if (Active)
            {
                var renderSpace = new StringWriter();
                Formatter.Format(logEvent, renderSpace);
                if (LogProperties)
                {
                    renderSpace.WriteLine(Environment.NewLine + string.Join(Environment.NewLine, logEvent.Properties.Select(x => "[" + x.Key + "," + x.Value + "]")));
                }
                var sse = new ServerSentEvent(MatchLevel(logEvent.Level), renderSpace.ToString());
                _channel.Send(sse, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
            }
        }

        private string MatchLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Verbose:
                    return "DEBUG";
                default:
                case LogEventLevel.Information:
                    return "INFO";
                case LogEventLevel.Warning:
                    return "WARN";
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return "ERROR";
            }
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }
        }
    }
}