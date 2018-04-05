using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false)]
#if NETFX_CORE
    public class SerializableAttribute : Attribute
    {
        public SerializableAttribute()
        {
        }
    }

    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false)]
    public class DataContractAttribute : Attribute
    {
        public DataContractAttribute()
        {
        }
    }

    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = true)]
    public class DataMemberAttribute : Attribute
    {
        public DataMemberAttribute()
        {
        }
    }
#endif
}
