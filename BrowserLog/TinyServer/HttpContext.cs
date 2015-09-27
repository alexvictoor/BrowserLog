using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class HttpContext
    {
        private readonly HttpRequest _httpRequest;
        private readonly HttpResponseChannel _responseChannel;

        public HttpContext(HttpRequest httpRequest, HttpResponseChannel responseChannel)
        {
            _httpRequest = httpRequest;
            _responseChannel = responseChannel;
        }

        public HttpRequest HttpRequest
        {
            get { return _httpRequest; }
        }

        public HttpResponseChannel ResponseChannel
        {
            get { return _responseChannel; }
        }
    }
}
