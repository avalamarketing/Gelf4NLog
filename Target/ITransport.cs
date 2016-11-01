using System.Net;

namespace Gelf4NLog.Target
{
    public interface ITransport
    {
        void Send(IPEndPoint target, string message);
    }
}
