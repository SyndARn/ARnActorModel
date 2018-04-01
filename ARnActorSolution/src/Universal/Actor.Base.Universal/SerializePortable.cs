using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
#if NETFX_CORE
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate,
    Inherited = false)]
    public sealed class SerializableAttribute : Attribute
    {
        public SerializableAttribute()
        {
        }
    }
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum,
    Inherited = false, AllowMultiple = false)]
    public sealed class DataContractAttribute : Attribute
    {
        public DataContractAttribute()
        {
        }
    }
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false, AllowMultiple = false)]
    public sealed class DataMemberAttribute : Attribute
    {
        public DataMemberAttribute()
        {
        }
    }
#endif
}
