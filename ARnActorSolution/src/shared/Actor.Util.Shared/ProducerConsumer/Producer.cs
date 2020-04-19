using System;
using System.Collections.Generic;
using System.Diagnostics;
using Actor.Base;

namespace Actor.Util
{
    public class Producer<T> : BaseActor
    {
        private readonly Buffer<T> _buffer;

        public Producer(Buffer<T> buffer) : base()
        {
            _buffer = buffer;
            Become(new Behavior<T>(DoProduce));
        }

        protected void DoProduce(T t)
        {
            var work = new Work<T>(t);
            _buffer.SendMessage(work);
        }
    }
}


