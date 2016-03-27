using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Server;
using Actor.Util;

namespace PrimeSumNumber_Euler543

{

    public delegate Task<long> Fn(long k);

    public class Fibonacci : BaseActor
    {
        public Fibonacci()
        {
            Become(new Behavior<Tuple<IActor, long>>(CalcIt));
        }

        public async Task<long> Calc(long k)
        {
            IEnumerable<Task<long>> list;
            switch (k)
            {
                case 0: return 0;
                case 1: return 1;
                default:
                    {
                        var fnminus1 = new Fibonacci();
                        fnminus1.SendMessage(new Tuple<IActor, long>(this, k - 1));
                        var r1 = Receive(t => t is long);

                        var fnminus2 = new Fibonacci();
                        fnminus2.SendMessage(new Tuple<IActor, long>(this, k - 2));
                        var r2 = Receive(t => t is long);

                        return (long)await r1 + (long)await r2;
                    }
            }
        }

        private void CalcIt(Tuple<IActor, long> msg)
        {
            long sum = Calc(msg.Item2).Result;
            msg.Item1.SendMessage(sum);
        }

    }
}
