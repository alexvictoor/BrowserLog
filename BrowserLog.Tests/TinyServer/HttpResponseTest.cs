using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;

namespace BrowserLog.TinyServer
{
    public class HttpResponseTest
    {
        [Test]
        public void Should_build_response_with_status_code()
        {
            // given
            var response = new HttpResponse(404);
            // when
            var textResponse = response.ToString();
            // then
            Check.That(textResponse).StartsWith("HTTP/1.1 404");
        }

        [Test]
        public void Should_build_response_with_headers()
        {
            // given
            var response = new HttpResponse(404);
            response.AddHeader("DummyHeader", "DummyValue");
            // when
            var textResponse = response.ToString();
            // then
            Check.That(textResponse).Contains("DummyHeader: DummyValue");
        }

        [Test]
        public void Shoud()
        {
            var multicast = new MulticastChannel();

            Action<HttpContext> handler = ctx =>
            {

                Console.Out.WriteLine("Before read request");
           
                var httpResponse = new HttpResponse(200);
                if (ctx.HttpRequest.Uri != "/stream")
                {
                    httpResponse.AddHeader("Content-Type", "text/html");
                    httpResponse.AddHeader("Connection", "close");
                    httpResponse.Content = "<html><body><h1>Coucou</h1></body></html>";
                    ctx.ResponseChannel.Send(httpResponse);
                }
                else
                {
                    httpResponse.AddHeader("Content-Type", "text/event-stream");
                    httpResponse.AddHeader("Cache-Control", "no-cache");
                    httpResponse.AddHeader("Connection", "keep-alive");
                    httpResponse.AddHeader("Access-Control-Allow-Origin", "*");
                    ctx.ResponseChannel.Send(httpResponse);
                    multicast.AddChannel(ctx.ResponseChannel);
                }
            };

            new HttpServer("127.0.0.1", 8081, handler).Run();
            Console.Out.WriteLine("Ready");
            for (int i = 0; i < 4000; i++)
            {
                Console.Out.WriteLine("Sending msg " + i);
                multicast.Send("id: " + i + "\ndata: " + DateTime.Now + "\n\n");
                Thread.Sleep(1000);
            }
            Console.In.Read();
        
        }
    }
}
