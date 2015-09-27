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
        private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();
        public string Content;

        public HttpResponse()
        {
            _statusCode = 0;
        }

        public HttpResponse(int statusCode)
        {
            _statusCode = statusCode;
        }

        public void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }

        public override string ToString()
        {
            var builder = new StringBuilder("HTTP/1.1 " + _statusCode + "\n");
            foreach (var header in _headers)
            {
                builder.Append(header.Key + ": " + header.Value + "\n");
            }
            builder.Append("\n");
            if (Content != null)
            {
                builder.Append(Content);
            }
            Console.Out.WriteLine("Sending;\n" + builder.ToString());
            return builder.ToString();
        }
    }
}
