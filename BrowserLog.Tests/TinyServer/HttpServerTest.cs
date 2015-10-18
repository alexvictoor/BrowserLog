using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;

namespace BrowserLog.TinyServer
{
    public class HttpServerTest
    {
        [Test]
        public async void Should_call_handler_on_request()
        {
            // given
            var port = FindFreeTcpPort();
            var server = BuildServer(port, "It works");
            // when
            var task = Task.Run(() => server.Run());
            
            // then
            var httpClient = new HttpClient();
            var urlPrefix = "http://localhost:" + port + "/";
            var content = await httpClient.GetStringAsync(urlPrefix);
            Check.That(content).Contains("It works");
        }

        [Test]
        public async void Should_stop_handling_requests_after_dispose()
        {
            // given
            var port = FindFreeTcpPort();
            var server = BuildServer(port, "It works");
            // when
            var serverTask = Task.Run(() => server.Run());
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            server.Dispose();

            // then
            var httpClient = new HttpClient();
            var urlPrefix = "http://localhost:" + port + "/";
            
            Check.ThatCode(() => httpClient.GetStringAsync(urlPrefix).Wait()).ThrowsAny();
        }

        [Test]
        public async void Should_throw_exception_running_a_disposed_server()
        {
            // given
            var port = FindFreeTcpPort();
            var server = BuildServer(port, "It works");
            // when
            server.Dispose();

            // then
            var serverTask = Task.Run(() => server.Run());
            Check.ThatCode(server.Run).Throws<InvalidOperationException>();
        }

        private static HttpServer BuildServer(int port, string content)
        {
            return new HttpServer("127.0.0.1", port, ctx =>
            {
                var response = new HttpResponse(200, "OK");
                response.AddHeader("Content-Type", "text/html; charset=utf-8");
                response.AddHeader("Connection", "close");
                response.AddHeader("Date", "Sun, 27 Sep 2015 20:19:46 GMT");
                response.Content = content;
                ctx.ResponseChannel.Send(response, CancellationToken.None);
            });
        }

        // see http://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net
        public static int FindFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
