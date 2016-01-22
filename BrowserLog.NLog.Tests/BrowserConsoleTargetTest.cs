using System;
using System.Linq;
using System.Threading;
using BrowserLog.TinyServer;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NSubstitute;
using NUnit.Framework;

namespace BrowserLog.NLog.Tests
{
    public class BrowserConsoleTargetTest
    {
        private IEventChannel _channel;
        private BrowserConsoleTarget _target;

        [SetUp]
        public void ConfigureLogger()
        {
            var layout = new SimpleLayout("${date} [${threadid}] ${level} ${logger} ${ndc} - ${message}${newline}");

            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create("localhost", 8765, 1).Returns(_channel);
            _target = new BrowserConsoleTarget(channelFactory)
            {
                Host = "localhost",
                Port = 8765,
                Layout = layout,
                Buffer = 1,
                Name = "ConsoleTest"
            };
        }

        [Test]
        public void Should_have_no_side_effect_if_active_flag_set_to_false()
        {
            // given
            ApplyTargetConfiguration();
            // when
            LogManager.GetLogger(_target.Name, GetType()).Info("Everything's fine with NLog");
            // then
            _channel.DidNotReceive().Send(Arg.Any<ServerSentEvent>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_an_sse_message_when_receiving_a_logging_event()
        {
            // given
            _target.Active = true;
            ApplyTargetConfiguration();
            // when
            LogManager.GetLogger(_target.Name, GetType()).Info("Everything's fine with NLog");
            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().Contains("Everything's fine with NLog")), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_an_sse_message_with_a_type_matching_received_logging_event_level()
        {
            // given
            _target.Active = true;
            ApplyTargetConfiguration();
            // when
            LogManager.GetLogger(_target.Name, GetType()).Warn("level?");
            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().StartsWith("event: WARN")), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_an_sse_message_without_type_when_received_logging_event_level_has_no_matching_level_on_browser()
        {
            // given
            _target.Active = true;
            ApplyTargetConfiguration();
            // when
            LogManager.GetLogger(_target.Name, GetType()).Fatal("No fatal logs on the browser");
            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().StartsWith("data:")), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_a_multiline_sse_message_received_logging_event_for_an_exception()
        {
            // given
            _target.Active = true;
            ApplyTargetConfiguration();
            // when
            LogManager.GetLogger(_target.Name, GetType()).Warn(new Exception(), "An error has occured");
            // then
            var lineSeparator = new string[] { "\r\n" };
            _channel.Received().Send(
                Arg.Is<ServerSentEvent>(
                    evt => evt.ToString()
                        .Split(lineSeparator, StringSplitOptions.RemoveEmptyEntries)
                        .Skip(1)
                        .All(l => l.StartsWith("data:"))
                    ), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_dispose_channel_on_shutdown()
        {
            // given
            _target.Active = true;
            ApplyTargetConfiguration();
            // when
            LogManager.Shutdown();
            // then
            _channel.Received().Dispose();
        }

        private void ApplyTargetConfiguration()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            LoggingRule rule = new LoggingRule("*", LogLevel.Debug, _target);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }
    }
}
