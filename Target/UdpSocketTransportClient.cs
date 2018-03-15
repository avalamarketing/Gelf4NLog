using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gelf4NLog.Target
{
    public class UdpSocketTransportClient : ITransportClient
    {
        public UdpSocketTransportClient()
        {
            SourcePort = 51515;
        }

        public void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint)
        {
            using (var udpClient = new UdpClient())
            {
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Bind(udpClient);
                udpClient.Send(datagram, bytes, ipEndPoint);
            }
        }

        private void Bind(UdpClient udpClient, int retryCount = 0)
        {
            if (retryCount >= 10)
                throw new InvalidOperationException("Exceeded max retry count to bind to a port.");

            try
            {
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, SourcePort + retryCount));
            }
            catch(SocketException sx)
            when (sx.ErrorCode == 10048)
            {
                Bind(udpClient, retryCount + 1);
            }
        }

        public int SourcePort { get; set; }
    }
}
