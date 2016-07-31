using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    
    [Serializable]
    public class ActorException : Exception
    {
        public ActorException()
            : base()
        { }

        public ActorException(string message)
            : base(message)
        { }

        public ActorException(String message, Exception inner) : base(message, inner) { }

#if !NETFX_CORE
        protected ActorException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
#endif

    }

}
