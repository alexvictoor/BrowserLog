using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrowserLog.TinyServer;

namespace BrowserLog
{
    public class ChannelFactory
    {
        public virtual IChannel Create(string host, int port)
        {
            var channel = new MulticastChannel();
            Action<HttpContext> handler = ctx =>
            {
                var httpResponse = new HttpResponse(200, "OK");
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
                    channel.AddChannel(ctx.ResponseChannel);
                }
            };
            new HttpServer(host, port, handler).Run();
            return channel;
        }
    }
}
