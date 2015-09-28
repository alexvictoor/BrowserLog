using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class ServerSentEvent
    {
        private readonly string _type;
        private readonly string _data;

        public ServerSentEvent(string type, string data)
        {
            _type = type;
            _data = data;
        }

        public override string ToString()
        {
            return "type: \"" + _type + "\"\r\n"
                   + "data: \"" + _data + "\"\r\n";
        }
    }
}
