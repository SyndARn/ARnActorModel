using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
