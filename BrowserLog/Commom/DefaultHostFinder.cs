using System.Net;
using System.Net.Sockets;

namespace BrowserLog.Commom
{
    public static class DefaultHostFinder
    {
        // hack described here
        // http://stackoverflow.com/a/27376368
        //
        private static string FindLocalIp()
        {
            string localIP;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }
    }
}