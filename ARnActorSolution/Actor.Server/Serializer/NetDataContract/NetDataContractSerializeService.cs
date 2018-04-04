﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public class NetDataContractSerializeService : ISerializeService
    {
        public void Serialize(object data, ActorTag aTag, Stream stream)
        {
            SerialObject so = new SerialObject(data, aTag);
            NetDataActorSerializer.Serialize(so, stream);
        }

        public object Deserialize(Stream stream)
        {
            return NetDataActorSerializer.DeSerialize(stream);
        }
    }

    public class DataContractSerializeService : ISerializeService
    {
        public void Serialize(object data, ActorTag aTag, Stream stream)
        {
            DataContractObject dco = new DataContractObject(data, aTag);
            DataContractActorSerializer.Serialize(dco, stream);
        }

        public object Deserialize(Stream stream)
        {
            return DataContractActorSerializer.DeSerialize(stream);
        }
    }
}