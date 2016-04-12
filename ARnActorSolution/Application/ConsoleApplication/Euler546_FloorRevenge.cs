using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PrimeSumNumber_Euler543
{

    public class Euler546 : BaseActor
    {
        BigInteger fK;
        BigInteger fSum;
        BigInteger entries;
        IActor fCaller;

        public Euler546(BigInteger aK) : base()
        {
            fK = aK;
            fSum = 0;
            Become(new Behavior<Tuple<IActor, BigInteger>>(Start));
        }


        private void Start(Tuple<IActor,BigInteger> msg)
        {
            fCaller = msg.Item1;
            Become(new Behavior<Tuple<IActor, BigInteger>>(WaitResult));

            BigInteger limit = BigInteger.Min(fK, msg.Item2) ;

            entries = 1;

            if (msg.Item2 >= fK)
            {
                entries += msg.Item2 - fK + 1;
            }
          
            SendMessage(new Tuple<IActor, BigInteger>(this, limit+1));


            for (BigInteger i = fK; i <= msg.Item2; i++)
            {
                BigInteger quote = i / fK;
                if (quote == 0)
                {
                    throw new Exception("bad");
                }
                else
                {
                    var act = new Euler546(fK);
                    act.SendMessage(new Tuple<IActor, BigInteger>(this, quote));
                }
            }

        }

        private void WaitResult(Tuple<IActor, BigInteger> msg)
        {
            entries--;
            fSum += msg.Item2;
            if (entries == 0)
                fCaller.SendMessage(new Tuple<IActor, BigInteger>(this, fSum));
        }
    }


}
