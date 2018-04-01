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

}
