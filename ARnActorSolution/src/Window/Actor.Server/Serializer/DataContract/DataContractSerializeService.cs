using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public class DataContractSerializeService : ISerializeService
    {
        public void Serialize(object data, ActorTag tag, Stream stream)
        {
            DataContractObject dco = new DataContractObject(data, tag);
            DataContractActorSerializer.Serialize(dco, stream);
        }

        public object Deserialize(Stream stream)
        {
            return DataContractActorSerializer.DeSerialize(stream);
        }
    }
}
