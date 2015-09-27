using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class HttpRequest
    {
        private readonly string _uri;
        private readonly string _method;
        private readonly IDictionary<string, string> _headers;

        public HttpRequest(string uri, string method, IDictionary<string, string> headers)
        {
            _uri = uri;
            _method = method;
            _headers = headers;
        }

        public string Uri
        {
            get { return _uri; }
        }

        public string Method
        {
            get { return _method; }
        }

        public IDictionary<string, string> Headers
        {
            get { return _headers; }
        }

        public static HttpRequest Parse(IEnumerable<string> lines)
        {
            string firstLine = lines.ElementAt(0);
            var spaceIndex = firstLine.IndexOf(' ');
            var lastSpaceIndex = firstLine.LastIndexOf(' ');
            var method = firstLine.Substring(0, spaceIndex).Trim().ToUpper();
            var uri = firstLine.Substring(spaceIndex, lastSpaceIndex - spaceIndex).Trim();
            var headers = new Dictionary<string, string>();
            foreach (var line in lines.Skip(1))
            {
                var separatorIndex = line.IndexOf(':');
                var headerName = line.Substring(0, separatorIndex).Trim();
                var headerValue = line.Substring(separatorIndex + 1).Trim();
                headers.Add(headerName, headerValue);
            }

            return new HttpRequest(uri, method, headers);
        }
    }
}
