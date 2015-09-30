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
            var response = new HttpResponse(404, "NOT FOUND");
            // when
            var textResponse = response.ToString();
            // then
            Check.That(textResponse).StartsWith("HTTP/1.1 404");
        }

        [Test]
        public void Should_build_response_with_headers()
        {
            // given
            var response = new HttpResponse(404, "NOT FOUND");
            response.AddHeader("DummyHeader", "DummyValue");
            // when
            var textResponse = response.ToString();
            // then
            Check.That(textResponse).Contains("DummyHeader: DummyValue");
        }

    }
}
