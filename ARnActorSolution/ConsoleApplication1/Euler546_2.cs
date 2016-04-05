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
    public class Euler546_2 : BaseActor
    {
        static BigInteger fK;
        BigInteger fN;
        BigInteger entries;
        IActor fCaller;
        BigInteger fSum;
        IActor fLast;
        private static ConcurrentDictionary<BigInteger, BigInteger> fDico;

        public Euler546_2(BigInteger aK) : base()
        {
            if (aK != fK)
            {
                fDico = null;
            }
            fK = aK;
            fSum = 0;
            if (fDico == null)
                fDico = new ConcurrentDictionary<BigInteger, BigInteger>();
            Become(new Behavior<Tuple<IActor, BigInteger>>(Start));
        }

        private BigInteger func(BigInteger n)
        {

            BigInteger remainder;
            BigInteger divrem = BigInteger.DivRem(n, fK, out remainder);

            for (BigInteger i = 0; i <= divrem - 1; i++)
            {
                if (i == 0)
                    fSum += fK;
                else
                if (i == 1)
                    fSum += 2 * fK;
                else
                {
                    BigInteger value;
                    if (fDico.TryGetValue(i, out value)) 
                    {
                        fSum += fK * value;
                    }
                    else
                    {

                        var eul = new Euler546_2(fK);
                        entries++;
                        eul.SendMessage(new Tuple<IActor, BigInteger>(this, i));
                    }
                }
            }

            if (divrem == 0)
                fSum += (remainder + 1);
            else
            if (divrem == 1)
                fSum += 2 * (remainder + 1);
            else
            {
                BigInteger value;
                if (fDico.TryGetValue(divrem, out value))
                {
                     fSum += (remainder + 1) * value;
                }
                else
                {
                    entries++;
                    fLast = new Euler546_2(fK);
                    fLast.SendMessage(new Tuple<IActor, BigInteger>(this, divrem));
                }
            }


            return fSum;
        }

        private void DoList(Tuple<IActor, BigInteger> msg)
        {
            entries--;
            if (msg.Item1 == fLast)
                fSum += msg.Item2 * (fN % fK +1);
            else 
               fSum += msg.Item2 * fK;
            if (entries == 0)
            {
                if (fN >= fK)
                  fDico[fN] = fSum;
                    // Console.WriteLine(string.Format("K {0} N {1} = {2}", fK, fN, fSum));
                fCaller.SendMessage(new Tuple<IActor, BigInteger>(this, fSum));
                
            }
        }

        private void Start(Tuple<IActor, BigInteger> msg)
        {
            Become(new Behavior<Tuple<IActor, BigInteger>>(DoList));
            fCaller = msg.Item1;
            fN = msg.Item2;
            fSum = func(fN);

            if (entries == 0)
            {
                if (fN >= fK)
                    fDico[fN] = fSum;
                    // Console.WriteLine(string.Format("K {0} N {1} = {2}", fK, fN, fSum));
               fCaller.SendMessage(new Tuple<IActor, BigInteger>(this, fSum));
                
            }

        }

    }
}
