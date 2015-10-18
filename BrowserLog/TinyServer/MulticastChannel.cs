using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class MulticastChannel : IEventChannel
    {
        private readonly IList<IEventChannel> _channels = new List<IEventChannel>();
        private readonly object _syncRoot = new object();
        private readonly int _replayBufferSize;
        private readonly IList<ServerSentEvent> _replayBuffer;
        private HttpServer _httpServer;

        public MulticastChannel(int replayBufferSize = 1)
        {
            _replayBufferSize = replayBufferSize;
            _replayBuffer = new List<ServerSentEvent>(replayBufferSize);
        }

        public void AddChannel(IEventChannel channel, CancellationToken token)
        {
            lock (_syncRoot)
            {
                _channels.Add(channel);
                foreach (var message in _replayBuffer)
                {
                    channel.Send(message, token);
                }
            }
        }

        public void Send(ServerSentEvent message, CancellationToken token)
        {
            lock (_syncRoot)
            {
                var closeChannels = new List<IEventChannel>();
                foreach (var channel in _channels)
                {
                    try
                    {
                        channel.Send(message, token);
                    }
                    catch (Exception)
                    {
                        closeChannels.Add(channel);
                    }
                }
                foreach (var channel in closeChannels)
                {
                    _channels.Remove(channel);
                }

                while (_replayBuffer.Count >= _replayBufferSize)
                {
                    _replayBuffer.RemoveAt(0);
                }
                _replayBuffer.Add(message);
            }
        }

        public void AttachServer(HttpServer httpServer)
        {
            _httpServer = httpServer;
        }

        public void Dispose()
        {
            if (_httpServer != null)
            {
                _httpServer.Dispose();   
            }
            foreach (var channel in _channels)
            {
                channel.Dispose();
            }
        }
    }
}
