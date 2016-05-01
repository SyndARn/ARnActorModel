using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public class MemoryContextComm : IContextComm
    {
        private Future<Stream> future = new Future<Stream>();

        public void Acknowledge()
        {
        }

        public Stream ReceiveStream()
        {
            return future.Result();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "clone stream is send as message")]
        public void SendStream(string uri, Stream stream)
        {
            CheckArg.Stream(stream);
            stream.Seek(0, SeekOrigin.Begin);
            MemoryStream clone = new MemoryStream();
            stream.CopyTo(clone);
            clone.Seek(0, SeekOrigin.Begin);
            future.SendMessage(clone);
        }
    }
}
