using System;
using System.IO;
using Actor.Base;

namespace Actor.Server
{
    public interface ISerializeService
    {
        void Serialize(Object data, ActorTag tag, Stream stream);
        Object Deserialize(Stream stream);
    }
}
