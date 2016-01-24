using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BrowserLog.Serilog;
using BrowserLog.TinyServer;
using Serilog;
using NSubstitute;
using NUnit.Framework;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;

namespace BrowserLog.Serilog.Tests
{
    public class BrowserConsoleSinkTest
    {
        private IEventChannel _channel;
        private BrowserConsoleSink _sink;

        [SetUp]
        public void ConfigureLogger()
        {
            var formatter = new MessageTemplateTextFormatter("${date} [${threadid}] ${level} ${logger} ${ndc} - ${message}${newline}", null);

            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create("localhost", 8765, 1).Returns(_channel);
            _sink = new BrowserConsoleSink(true, 8765, 1, formatter,false, channelFactory);
        }

        [Test]
        public void Should_have_no_side_effect_if_active_flag_set_to_false()
        {
            // given
            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create(Arg.Any<string>(), 8765, 1).Returns(_channel);
            var messageTemplateTextFormatter = Substitute.For<MessageTemplateTextFormatter>(BrowserConsoleLoggerConfigurationExtensions.DefaultOutputTemplate, null);
            var sink = new BrowserConsoleSink(false, 8765, 1, messageTemplateTextFormatter, false, channelFactory);

            var logEvent = new LogEvent(new DateTimeOffset(new DateTime(2010, 10, 12)), LogEventLevel.Information, new Exception("test"), GenerateMessageTemplate("Useless text"), new List<LogEventProperty>());

            // when
            sink.Emit(logEvent);

            // then
            messageTemplateTextFormatter.DidNotReceiveWithAnyArgs().Format(logEvent, new StringWriter());
        }

        [Test]
        public void Should_send_an_sse_message_when_receiving_a_logging_event()
        {
            // given
            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create(Arg.Any<string>(), 8765, 1).Returns(_channel);
            var messageTemplateTextFormatter = new MessageTemplateTextFormatter(BrowserConsoleLoggerConfigurationExtensions.DefaultOutputTemplate, null);
            var sink = new BrowserConsoleSink(true, 8765, 1, messageTemplateTextFormatter, false, channelFactory);

            var logEvent = new LogEvent(
                new DateTimeOffset(new DateTime(2010, 10, 12)),
                LogEventLevel.Information,
                null,
                GenerateMessageTemplate("Everything's fine with Serilog"),
                new List<LogEventProperty>() { new LogEventProperty("test", new ScalarValue("value"))});

            // when
            sink.Emit(logEvent);

            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().Contains("Everything's fine with Serilog")), Arg.Any<CancellationToken>());
        }
        
        private static MessageTemplate GenerateMessageTemplate(string text)
        {
            return new MessageTemplate(text, new List<MessageTemplateToken>()
            {
                new TextToken(text)
            });
        }

        [Test]
        public void Should_send_an_sse_message_with_a_type_matching_received_logging_event_level()
        {
            // given
            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create(Arg.Any<string>(), 8765, 1).Returns(_channel);
            var messageTemplateTextFormatter = new MessageTemplateTextFormatter(BrowserConsoleLoggerConfigurationExtensions.DefaultOutputTemplate, null);
            var sink = new BrowserConsoleSink(true, 8765, 1, messageTemplateTextFormatter, false, channelFactory);

            var logEvent = new LogEvent(
                new DateTimeOffset(new DateTime(2010, 10, 12)),
                LogEventLevel.Warning,
                null,
                GenerateMessageTemplate("Everything's fine with Serilog"),
                new List<LogEventProperty>() { new LogEventProperty("test", new ScalarValue("value")) });

            // when
            sink.Emit(logEvent);

            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().StartsWith("event: WARN")), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_an_sse_error_message_when_received_logging_event_level_similar_using_a_matching()
        {
            // given
            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create(Arg.Any<string>(), 8765, 1).Returns(_channel);
            var messageTemplateTextFormatter = new MessageTemplateTextFormatter(BrowserConsoleLoggerConfigurationExtensions.DefaultOutputTemplate, null);
            var sink = new BrowserConsoleSink(true, 8765, 1, messageTemplateTextFormatter, false, channelFactory);

            var logEvent = new LogEvent(
                new DateTimeOffset(new DateTime(2010, 10, 12)),
                LogEventLevel.Fatal,
                null,
                GenerateMessageTemplate("Fatal log is converted in error"),
                new List<LogEventProperty>() { new LogEventProperty("test", new ScalarValue("value")) });

            // when
            sink.Emit(logEvent);

            // then
            _channel.Received().Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().StartsWith("event: ERROR")), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_a_multiline_sse_message_received_logging_event_for_an_exception()
        {
            // given
            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create(Arg.Any<string>(), 8765, 1).Returns(_channel);
            var messageTemplateTextFormatter = new MessageTemplateTextFormatter(BrowserConsoleLoggerConfigurationExtensions.DefaultOutputTemplate, null);
            var sink = new BrowserConsoleSink(true, 8765, 1, messageTemplateTextFormatter, false, channelFactory);

            var logEvent = new LogEvent(
                new DateTimeOffset(new DateTime(2010, 10, 12)),
                LogEventLevel.Fatal,
                new Exception("Message of the exception"), 
                GenerateMessageTemplate("Displaying of an exception"),
                new List<LogEventProperty>() { new LogEventProperty("test", new ScalarValue("value")) });

            // when
            sink.Emit(logEvent);

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
            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create(Arg.Any<string>(), 8765, 1).Returns(_channel);
            var sink = new BrowserConsoleSink(true, 8765, 1, null, false, channelFactory);

            //When
            sink.Dispose();

            // then
            _channel.Received().Dispose();
        }
    }
}
