using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class HttpResponse
    {
        private readonly int _statusCode;
        private readonly string _statusDescription;
        private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();
        public string Content;

        public HttpResponse()
        {
            _statusCode = 0;
        }

        public HttpResponse(int statusCode, string statusDescription)
        {
            _statusCode = statusCode;
            _statusDescription = statusDescription;
        }

        public void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }

        public override string ToString()
        {
            var builder = new StringBuilder("HTTP/1.1 " + _statusCode + " " + _statusDescription +"\r\n");

            if (Content != null)
            {
                var byteCount = Encoding.UTF8.GetByteCount(Content);
                _headers.Add("Content-Length", byteCount.ToString());
            }
            foreach (var header in _headers)
            {
                builder.Append(header.Key + ": " + header.Value + "\r\n");
            }
            builder.Append("\r\n");
            if (Content != null)
            {
                builder.Append(Content);
            }

            return builder.ToString();
        }
    }
}
