using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

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
}
