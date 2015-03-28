using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    static class NetDataActorSerializer
    {
        public static SerialObject DeSerialize(Stream inputStream)
        {
            inputStream.Seek(0, SeekOrigin.Begin);
            NetDataContractSerializer dcs = new NetDataContractSerializer();
            dcs.SurrogateSelector = new ActorSurrogatorSelector();
            dcs.Binder = new ActorBinder();
            return (SerialObject)dcs.ReadObject(inputStream);
        }

        public static void Serialize(SerialObject so, Stream outputStream)
        {
            NetDataContractSerializer dcs = new NetDataContractSerializer();
            dcs.SurrogateSelector = new ActorSurrogatorSelector();
            dcs.Binder = new ActorBinder();
            dcs.Serialize(outputStream, so);
        }
    }
}
