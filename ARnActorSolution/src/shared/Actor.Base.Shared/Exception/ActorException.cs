using System;
#if NETCOREAPP1_1
using System.Runtime.Serialization.Json ;
#else
using System.Runtime.Serialization;
#endif
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

#if !NETFX_CORE && !NETCOREAPP1_1
        protected ActorException(SerializationInfo si, StreamingContext sc) : base(si, sc) { }
#endif

    }
}
