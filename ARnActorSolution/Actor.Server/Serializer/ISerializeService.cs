using System;
using System.IO;
using Actor.Base;

namespace Actor.Server
{
    public interface ISerializeService
    {
        void Serialize(Object so, ActorTag tag, Stream stream);
        Object Deserialize(Stream stream);
    }
}
