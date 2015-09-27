using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class LineParser
    {

        public async Task<IEnumerable<string>> Parse(Stream input, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[0x1000];
            var result = new List<string>();
            var lineBuilder = new StringBuilder();
            bool reading = true;
            while (reading && !cancellationToken.IsCancellationRequested)
            {
                var bytesRead = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                var chars = Encoding.ASCII.GetChars(buffer, 0, bytesRead);
                var charsRead = chars.Length;
                for (int i = 0; i < charsRead; i++)
                {
                    char c = chars[i];
                    if (c == '\n')
                    {
                        var line = lineBuilder.ToString();
                        lineBuilder.Clear();
                        if (line == null || line.Length == 0)
                        {
                            reading = false;
                        }
                        else
                        {
                            result.Add(line);
                        }
                    }
                    else if (c == '\r')
                    {
                        // ignore
                    }
                    else
                    {
                        lineBuilder.Append(c);
                    }
                }
            }
            return result;
        }
    }
}
