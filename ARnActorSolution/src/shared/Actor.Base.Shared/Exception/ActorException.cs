using System;
using System.Runtime.Serialization;

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

        public ActorException(string message, Exception inner) : base(message, inner) { }

#if !NETFX_CORE
        protected ActorException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
#endif

    }

}
