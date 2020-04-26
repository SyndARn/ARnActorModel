using System.IO;
using System.Runtime.Serialization;
using Actor.Base;

namespace Actor.Server
{
    public static class NetDataActorSerializer
    {
        public static SerialObject DeSerialize(Stream inputStream)
        {
            CheckArg.Stream(inputStream);
            inputStream.Seek(0, SeekOrigin.Begin);
            NetDataContractSerializer dcs = new NetDataContractSerializer()
            {
                SurrogateSelector = new ActorSurrogatorSelector(),
                Binder = new ActorBinder()
            };
            return (SerialObject)dcs.ReadObject(inputStream);
        }

        public static void Serialize(SerialObject so, Stream outputStream)
        {
            NetDataContractSerializer dcs = new NetDataContractSerializer()
            {
                SurrogateSelector = new ActorSurrogatorSelector(),
                Binder = new ActorBinder()
            };
            dcs.Serialize(outputStream, so);
        }
    }
}
