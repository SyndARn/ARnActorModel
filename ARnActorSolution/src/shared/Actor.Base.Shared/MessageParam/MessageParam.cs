using System;
using System.Runtime.Serialization;
#if NETCOREAPP1_1
using System.Runtime.Serialization.Json ;
#endif

namespace Actor.Base
{
    [Serializable]
    [DataContract]
    public sealed class MessageParam<T1,T2> : IMessageParam<T1, T2>
    {
        [DataMember]
        public T1 Item1 { get; private set; }

        [DataMember]
        public T2 Item2 { get; private set; }

        public MessageParam(T1 at1, T2 at2)
        {
            Item1 = at1;
            Item2 = at2;
        }
    }

    [Serializable]
    [DataContract]
    public sealed class MessageParam<T1, T2, T3> : IMessageParam<T1, T2, T3>
    {
        [DataMember]
        public T1 Item1 { get; private set; }

        [DataMember]
        public T2 Item2 { get; private set; }

        [DataMember]
        public T3 Item3 { get; private set; }

        public MessageParam(T1 at1, T2 at2, T3 at3)
        {
            Item1 = at1;
            Item2 = at2;
            Item3 = at3;
        }
    }

    [Serializable]
    [DataContract]
    public sealed class MessageParam<T1, T2, T3, T4> : IMessageParam<T1, T2, T3, T4>
    {
        [DataMember]
        public T1 Item1 { get; private set; }

        [DataMember]
        public T2 Item2 { get; private set; }

        [DataMember]
        public T3 Item3 { get; private set; }

        [DataMember]
        public T4 Item4 { get; private set; }

        public MessageParam(T1 at1, T2 at2, T3 at3, T4 at4)
        {
            Item1 = at1;
            Item2 = at2;
            Item3 = at3;
            Item4 = at4;
        }
    }
}
