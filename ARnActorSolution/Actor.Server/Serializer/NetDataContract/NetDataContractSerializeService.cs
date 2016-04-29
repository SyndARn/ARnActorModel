using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server
{
    public class NetDataContractSerializeService : ISerializeService
    {
        public void Serialize(SerialObject so, Stream stream)
        {
            NetDataActorSerializer.Serialize(so, stream);
        }

        public SerialObject DeSerialize(Stream stream)
        {
            return NetDataActorSerializer.DeSerialize(stream);
        }
    }
}
