using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrowserLog.TinyServer;
using log4net;
using log4net.Config;
using log4net.Layout;
using NSubstitute;
using NUnit.Framework;

namespace BrowserLog
{
    public class BrowserConsoleAppenderTest
    {
        private IChannel _channel;
        private BrowserConsoleAppender _appender;

        [SetUp]
        public void ConfigureLogger()
        {
            var layout = new PatternLayout("%-4timestamp [%thread] %-5level %logger %ndc - %message%newline");

            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IChannel>();
            channelFactory.Create("localhost", 8765).Returns(_channel);
            _appender = new BrowserConsoleAppender(channelFactory)
            {
                Host = "localhost",
                Port = 8765,
                Layout = layout
            };
            layout.ActivateOptions();
            
        }

        [Test]
        public void Should_have_no_side_effect_if_active_flag_set_to_false()
        {
            // given
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);
            // when
            LogManager.GetLogger(GetType()).Info("Everything's fine");
            // then
            _channel.DidNotReceive().Send(Arg.Any<ServerSentEvent>());
        }

        [Test]
        public void Should_send_an_sse_message_when_receiving_a_logging_event()
        {
            // given
            _appender.Active = true;
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);
            // when
            LogManager.GetLogger(GetType()).Info("Everything's fine");
            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().Contains("Everything's fine")));
        }

        [Test]
        public void Should_send_an_sse_message_wih_a_type_matching_received_logging_event_level()
        {
            // given
            _appender.Active = true;
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);
            // when
            LogManager.GetLogger(GetType()).Warn("level?");
            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().StartsWith("event: WARN")));
        }
    }
}
