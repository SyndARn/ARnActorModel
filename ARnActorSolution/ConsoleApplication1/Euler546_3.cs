using Actor.Base;
using Actor.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PrimeSumNumber_Euler543
{
    public class Euler546_3 : BaseActor
    {
        BigInteger fK;

        public Euler546_3(BigInteger aK) : base()
        {
            fK = aK;
            Become(new Behavior<Tuple<IActor, BigInteger>>(Start));
        }

        private BigInteger func(BigInteger n)
        {
            BigInteger fSum = 0;
            if (n == 0)
                return 1;
            if (n == 1)
                return 2;
            BigInteger Remainder;
            BigInteger div = BigInteger.DivRem(n, fK, out Remainder);
            if (div == 0)
                return Remainder + 1;
            if (div == 1)
                return  fK + 2* (Remainder + 1);
            fSum = (Remainder + 1) * func(div) + 3 * fK;
            for(BigInteger i = 2;i<=div-1;i++)
              fSum += fK*func(div-1) ;
            return fSum;
        }

        private void Start(Tuple<IActor, BigInteger> msg)
        {
            IActor fCaller = msg.Item1;
            BigInteger fN = msg.Item2;
            BigInteger fSum = func(fN);
            fCaller.SendMessage(new Tuple<IActor, BigInteger>(this, fSum));
        }

    }
}

