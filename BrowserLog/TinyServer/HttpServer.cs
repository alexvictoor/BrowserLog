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
    public class HttpServer
    {
        private readonly int _port ;
        private readonly IPAddress _host;
        private readonly Func<HttpContext, Task> _handler;
        private volatile bool _running;

        public HttpServer(string hostName, int port, Func<HttpContext, Task> handler)
        {
            _port = port;
            _handler = handler;
            _host = IPAddress.Parse(hostName);
        }

        public async void Run()
        {
            var server = new TcpListener(_host, _port);

            // Start listening for client requests.
            server.Start();
            _running = true;
            while (_running)
            {
                var tcpClient = await server.AcceptTcpClientAsync();
                Task.Run(async () =>
                    {
                        Console.Out.WriteLine("connection received");
                        var source = tcpClient.GetStream();
                        var lines = await new LineParser().Parse(source, CancellationToken.None);
                        var httpRequest = HttpRequest.Parse(lines);
                        var responseChannel = new HttpResponseChannel(tcpClient);
                        var httpContext = new HttpContext(httpRequest, responseChannel);
                        _handler(httpContext).Wait();
                        Console.Out.WriteLine("End Handling request");
                    });
                
            }
        }
    }
}
