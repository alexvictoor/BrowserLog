using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class HttpContext
    {
        private readonly HttpRequest _httpRequest;
        private readonly HttpResponseChannel _responseChannel;
        private readonly CancellationToken _token;

        public HttpContext(HttpRequest httpRequest, HttpResponseChannel responseChannel, CancellationToken token)
        {
            _httpRequest = httpRequest;
            _responseChannel = responseChannel;
            _token = token;
        }

        public HttpRequest HttpRequest
        {
            get { return _httpRequest; }
        }

        public HttpResponseChannel ResponseChannel
        {
            get { return _responseChannel; }
        }

        public CancellationToken Token
        {
            get { return _token; }
        }
    }
}
