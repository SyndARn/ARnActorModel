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
    public class Fibonacci2 : BaseActor
    {

        List<IActor> list = new List<IActor>();
        long fSum = 0;

        public Fibonacci2()
        {
            Become(new Behavior<Tuple<IActor, long>>(CalcIt));
            AddBehavior(new Behavior<Tuple<IActor, long, long>>(FinishIt));
        }

        public async Task<long> Calc(long k)
        {
            SendMessage(new Tuple<IActor, long>(this, k));
            return (long)await Receive(t => t is long);
        }

        private void FinishIt(Tuple<IActor, long, long> msg)
        {
            list.Remove(msg.Item1);
            fSum += msg.Item3;
            if (list.Count == 0)
                msg.Item1.SendMessage(fSum);
        }

        private void CalcIt(Tuple<IActor, long> msg)
        {
            long sum = 0;
            if (list.Count > 0)
            {
                list.Remove(msg.Item1);
            }
            long k = msg.Item2;
            IActor act = msg.Item1;
            switch (k)
            {
                case 0: sum = 0; break;
                case 1: sum = 1; break;
                default:
                    {
                        var fnminus1 = new Fibonacci2();
                        var fnminus2 = new Fibonacci2();
                        list.Add(fnminus1);
                        list.Add(fnminus2);
                        fnminus1.SendMessage(new Tuple<IActor, long>(this, k - 1));
                        fnminus2.SendMessage(new Tuple<IActor, long>(this, k - 2));
                        break;
                    }
            }
            msg.Item1.SendMessage(new Tuple<IActor, long, long>(this, k, sum));
        }
    }

}
