using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public static class DataContractActorSerializer
    {
        public static DataContractObject DeSerialize(Stream inputStream)
        {
            CheckArg.Stream(inputStream);
            inputStream.Seek(0, SeekOrigin.Begin);
            IDataContractSurrogate dataContractSurrogate = new DataContractActorSurrogate();
            var dcs = new DataContractSerializer(typeof(DataContractObject), new Type[] { typeof(ActorTag), typeof(DataContractObject), typeof(BaseActor) }, 1000, true, true, dataContractSurrogate);
            return (DataContractObject)dcs.ReadObject(inputStream);
        }

        public static void Serialize(DataContractObject so, Stream outputStream)
        {
            IDataContractSurrogate dataContractSurrogate = new DataContractActorSurrogate();
            var dcs = new DataContractSerializer(typeof(DataContractObject), new Type[] { typeof(ActorTag), typeof(DataContractObject), typeof(BaseActor) }, 1000, true, true, dataContractSurrogate);
            dcs.WriteObject(outputStream, so);
        }
    }
}
