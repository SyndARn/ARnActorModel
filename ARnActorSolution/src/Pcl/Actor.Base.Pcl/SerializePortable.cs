using System;

namespace Actor.Base
{
#if NETFX_CORE
    public class SerializableAttribute : Attribute
    {
        public SerializableAttribute()
        {
        }
    }
    public class DataContractAttribute : Attribute
    {
        public DataContractAttribute()
        {
        }
    }
    public class DataMemberAttribute : Attribute
    {
        public DataMemberAttribute()
        {
        }
    }
#endif
}
