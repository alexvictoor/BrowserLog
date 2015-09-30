using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public interface IChannel
    {
        void Send(ServerSentEvent message);
    }
}
