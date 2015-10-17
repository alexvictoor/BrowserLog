using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class ServerSentEvent
    {
        
        private readonly static string[] LogLevels = {"DEBUG", "INFO", "WARN", "ERROR"};
        private readonly string _type;
        private readonly string _data;

        public ServerSentEvent(string type, string data)
        {
            _type = type;
            _data = data;
        }

        public override string ToString()
        {
            var lines = _data.Split(new string[] {"\r\n"}, StringSplitOptions.None);
            var builder = new StringBuilder();
            if (LogLevels.Contains(_type))
            {
                builder.Append("event: " + _type + "\r\n");
            }
            foreach (var line in lines)
            {
                builder.Append("data: " + line + "\r\n");
            }
            builder.Append("\r\n");
            return builder.ToString();
        }
    }
}
