using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class MulticastChannel : IChannel
    {
        private readonly IList<IChannel> _channels = new List<IChannel>();
        private readonly object _syncRoot = new object();

        public void AddChannel(IChannel channel)
        {
            lock (_syncRoot)
            {
                _channels.Add(channel);
            }
        }

        public void Send(ServerSentEvent message)
        {
            lock (_syncRoot)
            {
                var closeChannels = new List<IChannel>();
                foreach (var channel in _channels)
                {
                    try
                    {
                        channel.Send(message);
                    }
                    catch (Exception ex)
                    {
                        closeChannels.Add(channel);
                    }
                }
                foreach (var channel in closeChannels)
                {
                    _channels.Remove(channel);
                }
            }
        }

    }
}
