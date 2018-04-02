using System.IO;

namespace Actor.Server
{ 
    public interface IContextComm
    {
        Stream ReceiveStream();
        void Acknowledge();
        void SendStream(string host, Stream stream);
    }
}
