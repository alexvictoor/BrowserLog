using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace BrowserLog.TinyServer
{
    public class MulticastChannelTest
    {
        [Test]
        public void Should_send_event_to_added_channel()
        {
            // given
            var multicastChannel = new MulticastChannel();
            var channel = Substitute.For<IEventChannel>();
            multicastChannel.AddChannel(channel);
            // when
            multicastChannel.Send(new ServerSentEvent("DEBUG", "whatever"));
            // then
            channel.Received().Send(Arg.Any<ServerSentEvent>());
        }

        [Test]
        public void Should_stop_sending_events_on_a_close_channel()
        {
            // given
            var multicastChannel = new MulticastChannel();
            var channel = Substitute.For<IEventChannel>();
            channel.When(c => c.Send(Arg.Any<ServerSentEvent>())).Do(x => { throw new Exception(); });
            multicastChannel.AddChannel(channel);
            // when
            multicastChannel.Send(new ServerSentEvent("DEBUG", "whatever")); // exception raised
            multicastChannel.Send(new ServerSentEvent("DEBUG", "whatever")); // channel should be removed
            // then
            channel.Received(1).Send(Arg.Any<ServerSentEvent>());
        }

        [Test]
        public void Should_replay_last_event_when_adding_a_channel()
        {
            // given
            var multicastChannel = new MulticastChannel();
            var channel = Substitute.For<IEventChannel>();
            multicastChannel.Send(new ServerSentEvent("DEBUG", "whatever"));
            // when
            multicastChannel.AddChannel(channel);
            // then
            channel.Received().Send(Arg.Any<ServerSentEvent>());
        }

        [Test]
        public void Should_replay_only_last_events_when_adding_a_channel()
        {
            // given
            var multicastChannel = new MulticastChannel(2);
            var channel = Substitute.For<IEventChannel>();
            multicastChannel.Send(new ServerSentEvent("DEBUG", "first"));
            multicastChannel.Send(new ServerSentEvent("DEBUG", "whatever2"));
            multicastChannel.Send(new ServerSentEvent("DEBUG", "whatever3"));
            // when
            multicastChannel.AddChannel(channel);
            // then
            channel.Received(2).Send(Arg.Any<ServerSentEvent>());
            channel.DidNotReceive().Send(Arg.Is<ServerSentEvent>(e => e.ToString().Contains("first")));
        }
    }
}
