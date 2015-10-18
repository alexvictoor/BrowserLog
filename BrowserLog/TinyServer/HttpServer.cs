using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class HttpServer : IDisposable
    {
        private readonly int _port ;
        private readonly IPAddress _host;
        private readonly Action<HttpContext> _handler;
        private volatile bool _running;
        private volatile bool _disposed;
        private TcpListener _server;

        public HttpServer(string ip, int port, Action<HttpContext> handler)
        {
            _port = port;
            _handler = handler;
            _host = IPAddress.Parse(ip);
        }

        public void Run()
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Cannot run disposed server");
            }
            _server = new TcpListener(_host, _port);

            // Start listening for client requests.
            _server.Start();
            _running = true;
            Task.Run(async () =>
            {
                while (_running)
                {
                    var tcpClient = await _server.AcceptTcpClientAsync();
                    var cancelTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // todo configuration
                    var task = Task.Run(async () =>
                    {
                        var source = tcpClient.GetStream();
                        var lines = await new LineParser().Parse(source, cancelTokenSource.Token);
                        var httpRequest = HttpRequest.Parse(lines);
                        var responseChannel = new HttpResponseChannel(tcpClient);
                        var httpContext = new HttpContext(httpRequest, responseChannel, cancelTokenSource.Token);
                        _handler(httpContext);
                    }, cancelTokenSource.Token);
                }
            });
        }

        public void Dispose()
        {
            _disposed = true;
            _running = false;
            if (_server != null)
            {
                _server.Stop();    
            }
        }
    }
}
