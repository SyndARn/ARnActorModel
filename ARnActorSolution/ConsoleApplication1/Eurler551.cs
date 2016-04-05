using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeSumNumber_Euler543
{

    /*
Let a0, a1, a2, ... be an integer sequence defined by:
•a0 = 1;
•for n ≥ 1, an is the sum of the digits of all preceding terms.

The sequence starts with 1, 1, 2, 4, 8, 16, 23, 28, 38, 49, ...
 You are given a10pow6 = 31054319.

Find a10pow15.

        

    */

    public class Eurler551 : BaseActor
    {
        public Eurler551() : base()
        {
            Become(new Behavior<long>(DoApply));
        }

        public async Task<long> Calc(long n)
        {
            SendMessage(n);
            var r = await Receive(t =>
            {
                var msg = t as Tuple<IActor, long>;
                return msg != null;
            });
            return ((Tuple<IActor, long>)r).Item2;
        }

        private void DoApply(long n)
        {
            long sum = 0;
            if (n <= 1)
            {
                sum = 1;
            }
            else
            {
                var an = new Eurler551();
                var prec = an.Calc(n - 1).Result ;
                sum = prec ;
                while (prec > 0)
                {
                    sum += prec % 10;
                    prec = prec / 10 ;
                }
            }
            SendMessage(new Tuple<IActor, long>(this, sum));
        }
    }


}
