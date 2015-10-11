using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrowserLog.TinyServer;
using NFluent;
using NUnit.Framework;

namespace BrowserLog.TinyServer
{
    public class LineParserTest
    {
        [Test]
        [Timeout(100)]
        public async void Should_parse_one_line()
        {
            // given
            byte[] buffer = Encoding.ASCII.GetBytes("first line\n\n");
            var stream = new MemoryStream(buffer);
            var parser = new LineParser();
            // when
            var lines = await parser.Parse(stream, CancellationToken.None);
            // then 
            Check.That(lines).HasSize(1);
            Check.That(lines.ElementAt(0)).IsEqualTo("first line");
        }

        [Test]
        public async void Should_parse_two_lines()
        {
            // given
            byte[] buffer = Encoding.ASCII.GetBytes("first line\nsecond line\n\n");
            var stream = new MemoryStream(buffer);

            var parser = new LineParser();
            // when
            var lines = await parser.Parse(stream, CancellationToken.None);
            // then 
            Check.That(lines).HasSize(2);
            Check.That(lines.ElementAt(0)).IsEqualTo("first line");
            Check.That(lines.ElementAt(1)).IsEqualTo("second line");
        }

        [Test]
        [Timeout(2000)]
        public async void Should_parse_lines_till_cancelled()
        {
            // given
            byte[] buffer = Encoding.ASCII.GetBytes("first line\nsecond");
            var stream = new MemoryStream(buffer);

            var parser = new LineParser();
            // when
            var source = new CancellationTokenSource();
            var parsingTask = Task.Run(() => parser.Parse(stream, source.Token));
            await Task.Delay(200);
            source.Cancel();
            await Task.Delay(1000);
            // then
            Check.That(parsingTask.Status).IsNotEqualTo(TaskStatus.Running);
        }
    }
}
