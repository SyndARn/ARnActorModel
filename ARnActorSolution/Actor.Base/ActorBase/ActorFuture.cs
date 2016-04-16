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

    public class Future<T1,T2> : BaseActor
    {
        public Future()
        {
        }

        public Tuple<T1,T2> Result() => (Tuple<T1, T2>)Receive(t => t is Tuple<T1, T2>).Result;

        public Tuple<T1, T2> Result(int timeOutMs) => (Tuple<T1, T2>)Receive(t => t is Tuple<T1, T2>, timeOutMs).Result;

    }
}
