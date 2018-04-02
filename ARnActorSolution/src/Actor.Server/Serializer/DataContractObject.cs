using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Runtime.Serialization;

namespace Actor.Server
{
    [DataContract]
    public class DataContractObject
    {
        [DataMember]
        public object Data { get; private set; }
        [DataMember]
        public ActorTag Tag { get; private set; }

        public DataContractObject() { }
        public DataContractObject(object someData, ActorTag aTag)
        {
            Data = someData;
            Tag = aTag;
        }
    }
}
