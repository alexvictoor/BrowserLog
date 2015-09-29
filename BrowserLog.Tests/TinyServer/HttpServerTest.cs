using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;

namespace BrowserLog.TinyServer
{
    public class HttpServerTest
    {
        [Test]
        public async void Should_Call_Handler_on_Request()
        {
            // given
            var port = FindFreeTcpPort();
            bool called = false;
            var server = new HttpServer("127.0.0.1", port, ctx =>
            {
                var response = new HttpResponse(200, "OK");
                response.AddHeader("Content-Type", "text/html; charset=utf-8");
                response.AddHeader("Connection", "close");
                response.AddHeader("Date", "Sun, 27 Sep 2015 20:19:46 GMT");
                response.Content = "It works";
                ctx.ResponseChannel.Send(response);
                //ctx.ResponseChannel.Close();
            });
            // when
            Task.Run(() => server.Run());
            
            // then
            var httpClient = new HttpClient();
            var urlPrefix = "http://localhost:" + port + "/";
            Console.Out.WriteLine("server " + urlPrefix);
            var content = await httpClient.GetStringAsync(urlPrefix);
            Check.That(content).Contains("It works");
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
