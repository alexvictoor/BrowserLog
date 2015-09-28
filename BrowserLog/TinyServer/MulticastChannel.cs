using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class MulticastChannel
    {
        private IList<HttpResponseChannel> _channels = new List<HttpResponseChannel>();
        private readonly object _syncRoot = new object();

        public void AddChannel(HttpResponseChannel channel)
        {
            lock (_syncRoot)
            {
                _channels.Add(channel);
            }
        }

        public void Send(object message)
        {
            lock (_syncRoot)
            {
                var closeChannels = new List<HttpResponseChannel>();
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
