using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
#if NETFX_CORE
    [AttributeUsage(AttributeTargets.All)]
    public sealed class SerializableAttribute : Attribute
    {
        public SerializableAttribute()
        {
        }
    }
#endif
}
