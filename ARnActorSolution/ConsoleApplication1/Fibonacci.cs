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
    public class Fibonacci : BaseActor
    {
        public Fibonacci()
        {
            Become(new Behavior<Tuple<IActor,long>>(CalcIt)) ;
        }

        public long Calc(long k)
            {
                switch(k)
            {
                case 0: return 0;
                case 1: return 1;
                default:
                    {
                        var fnminus1 = new Fibonacci();
                        var fnminus2 = new Fibonacci();
                        fnminus1.SendMessage(new Tuple<IActor,long>(this,k - 1));
                        fnminus2.SendMessage(new Tuple<IActor,long>(this,k - 2));
                        var r1 = Receive(t => t is long) ;
                        var r2 = Receive(t => t is long) ;
                        return (long)r1.Result + (long)r2.Result;
                    }
            }
            }

        private void CalcIt(Tuple<IActor,long> msg)
        {
            long sum = Calc(msg.Item2);
            msg.Item1.SendMessage(sum);
        }
        
    }
}
