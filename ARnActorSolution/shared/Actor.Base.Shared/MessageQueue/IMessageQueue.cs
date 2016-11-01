using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public interface IMessageQueue<T>
    {
        void Add(T item);
        bool TryTake(out T item);
        int Count();
    }
}
