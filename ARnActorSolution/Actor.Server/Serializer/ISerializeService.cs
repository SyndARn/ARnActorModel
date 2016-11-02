using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Actor.Server
{
    public interface ISerializeService
    {
        void Serialize(SerialObject so, Stream stream);
        SerialObject Deserialize(Stream stream);
    }
}
