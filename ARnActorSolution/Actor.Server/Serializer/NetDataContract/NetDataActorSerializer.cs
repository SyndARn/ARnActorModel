using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Procurios.Public;

namespace Actor.Server
{

    public static class NetDataActorSerializer
    {
        public static SerialObject DeSerialize(Stream inputStream)
        {
            CheckArg.Stream(inputStream);
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

    public static class DataContractActorSerializer
    {
        public static DataContractObject DeSerialize(Stream inputStream)
        {
            CheckArg.Stream(inputStream);
            inputStream.Seek(0, SeekOrigin.Begin);
            IDataContractSurrogate dataContractSurrogate = new DataContractActorSurrogate();
            DataContractSerializer dcs = new DataContractSerializer(typeof(DataContractObject),null,1000,true,true,dataContractSurrogate);
            return (DataContractObject)dcs.ReadObject(inputStream);
        }

        public static void Serialize(DataContractObject so, Stream outputStream)
        {
            IDataContractSurrogate dataContractSurrogate = new DataContractActorSurrogate();
            DataContractSerializer dcs = new DataContractSerializer(typeof(DataContractObject), null, 1000, true, true, dataContractSurrogate);
            dcs.WriteObject(outputStream, so);
        }
    }
}
