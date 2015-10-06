using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using BrowserLog.TinyServer;

namespace BrowserLog
{
    public class ChannelFactory
    {
        public virtual IEventChannel Create(string host, int port)
        {
            var channel = new MulticastChannel();
            var resourceManager = new ResourceManager("BrowserLog.Static", Assembly.GetExecutingAssembly());
            var js = resourceManager.GetObject("BrowserLog") as string;
            var htmlTemplate = resourceManager.GetObject("homepage") as string;
            var html = htmlTemplate.Replace("HOST", Dns.GetHostName()).Replace("PORT", port.ToString());

            Action<HttpContext> handler = ctx =>
            {
                var httpResponse = new HttpResponse(200, "OK");
                if (ctx.HttpRequest.Uri == "/")
                {
                    httpResponse.AddHeader("Content-Type", "text/html");
                    httpResponse.AddHeader("Connection", "close");
                    httpResponse.Content = html;
                    ctx.ResponseChannel.Send(httpResponse, ctx.Token).ContinueWith(t => ctx.ResponseChannel.Close());
                } 
                else if (ctx.HttpRequest.Uri == "/BrowserLog.js")
                {
                    httpResponse.AddHeader("Content-Type", "text/javascript");
                    httpResponse.AddHeader("Connection", "close");
                    httpResponse.Content = js;
                    ctx.ResponseChannel.Send(httpResponse, ctx.Token).ContinueWith(t => ctx.ResponseChannel.Close());
                }
                else
                {
                    httpResponse.AddHeader("Content-Type", "text/event-stream");
                    httpResponse.AddHeader("Cache-Control", "no-cache");
                    httpResponse.AddHeader("Connection", "keep-alive");
                    httpResponse.AddHeader("Access-Control-Allow-Origin", "*");
                    ctx.ResponseChannel.Send(httpResponse, ctx.Token)
                        .ContinueWith(t => ctx.ResponseChannel.Send(new ServerSentEvent("INFO", "Connected successfully on LOG stream from " + host + ":" + port )));
                    channel.AddChannel(ctx.ResponseChannel);
                }
            };
            new HttpServer(host, port, handler).Run();
            return channel;
        }
    }
}
