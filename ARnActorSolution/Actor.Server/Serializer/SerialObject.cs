using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.IO;

namespace Actor.Server
{
    [Serializable]
    public class SerialObject
    {
        public Object Data { get; private set; }
        public ActorTag Tag { get; private set; }
        public SerialObject() { }
        public SerialObject(object someData, ActorTag aTag)
        {
            Data = someData;
            Tag = aTag;
        }
    }

    public interface ISerializeService
    {
        void Serialize(SerialObject so, Stream stream);
        SerialObject DeSerialize(Stream stream);
    }

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
