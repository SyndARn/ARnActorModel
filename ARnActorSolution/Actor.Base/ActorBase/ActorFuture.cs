using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class Future<T> : BaseActor
    {
        public Future()
        {
        }

        public T Result() => (T)Receive(t => t is T).Result;

        public T Result(int timeOutMs) => (T)Receive(t => t is T, timeOutMs).Result;

    }
}
