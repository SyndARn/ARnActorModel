using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Actor.Base
{
    // 1 consumer
    // many producer
    // large ring buffer (1024)
    // token

    public class BufferQueue<T> : IMessageQueue<T>
    {
        private int _token;
        private T[] _list = new T[128];
        private int _read;

        public void Add(T item)
        {
            // check current tokenThreadId add with tokenHead until maxTokenHead
            // update tokenHead
            var myToken = Interlocked.Increment(ref _token);
            _list[myToken] = item;
        }

        public int Count()
        {
            return 0;
        }

        public bool TryTake(out T item)
        {
            if ((_read+1) <= Interlocked.CompareExchange(ref _token,_token,_token))
            {
                _read++;
                item =  _list[_read];
                return true;
            }
            item = default;
            return false;
        }
    }
}

