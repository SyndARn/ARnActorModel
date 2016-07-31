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

        public T Result(int timeOutMS) => (T)Receive(t => t is T, timeOutMS).Result;

        public async Task<T> ResultAsync()
        {
            return (T)await Receive(t => t is T);
        }

    }

    public class Future<T1,T2> : BaseActor
    {
        public Future()
        {
        }

        public Tuple<T1,T2> Result() => (Tuple<T1, T2>)Receive(t => t is Tuple<T1, T2>).Result;

        public Tuple<T1, T2> Result(int timeOutMS) => (Tuple<T1, T2>)Receive(t => t is Tuple<T1, T2>, timeOutMS).Result;

        public async Task<Tuple<T1, T2>> ResultAsync()
        {
            return (Tuple < T1, T2 >) await Receive(t => t is Tuple<T1, T2>);
        }

    }
}
