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

    public class Fibonacci3 : BaseActor
    {
        long fSum;
        IActor fCaller;
        int entries;
        public Fibonacci3()
        {
            Become(new Behavior<Tuple<IActor, long>>(Start));
        }

        private void Start(Tuple<IActor, long> msg)
        {
            fSum = 0;
            fCaller = msg.Item1;
            Become(new Behavior<Tuple<IActor, long>>(WaitResult));
            switch (msg.Item2)
            {
                case 0:
                    fCaller.SendMessage(new Tuple<IActor, long>(this, 0)) ; break;
                case 1:
                    fCaller.SendMessage(new Tuple<IActor, long>(this, 1)); break;
                default:
                    {
                        entries += 2;
                        var fnminus1 = new Fibonacci3();
                        fnminus1.SendMessage(new Tuple<IActor, long>(this, msg.Item2 - 1));

                        var fnminus2 = new Fibonacci3();
                        fnminus2.SendMessage(new Tuple<IActor, long>(this, msg.Item2 - 2));
                        break;
                    }
            }
        }

        private void WaitResult(Tuple<IActor, long> msg)
        {
            fSum += msg.Item2;
            entries--;
            if (entries == 0)
              fCaller.SendMessage(new Tuple<IActor, long>(this, fSum)) ;
        }

    }
}
