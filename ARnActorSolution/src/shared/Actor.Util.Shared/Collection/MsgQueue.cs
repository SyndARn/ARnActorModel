using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Util
{
    public interface IMsgQueue<T>
    {
        bool Result { get; }
        T Data { get; }
    }

    public class MsgQueue<T> : IMsgQueue<T>
    {
        public bool Result { get; }
        public T Data { get; }
        public MsgQueue(bool aResult, T aData)
        {
            Result = aResult;
            Data = aData;
        }
    }
}
