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
    public class Euler546Message
    {
        public IActor Sender;
        public BigInteger Key;
        public BigInteger Number;
        public BigInteger Sum;
    }

    public class Receiver : BaseActor
    {
        public Euler546Message Call(IActor sender,BigInteger key, BigInteger number)
        {
            var r = Receive(t => t is Euler546Message);

            sender.SendMessage(new Tuple<IActor, BigInteger, BigInteger>(this, key, number));

            return (Euler546Message)r.Result ;

        }
    }

    public class Euler546_3 : BaseActor
    {
        public Euler546_3() : base()
        {
            Become(new Behavior<Tuple<IActor, BigInteger, BigInteger>>(DoCalc));
        }


        private BigInteger func2(BigInteger key, BigInteger number)
        {

            BigInteger remainder;
            BigInteger key2 = key * key;
            BigInteger div = BigInteger.DivRem(number, key2, out remainder);
            BigInteger sum = 0;

            for (BigInteger i = 0; i <= div - 1; i++)
            {
                if (i == 0)
                    sum += key;
                else
                if (i == 1)
                    sum += 2 * key;
                else
                {
                    sum += func2(key, i) * key;
                }
            }

            if (div == 0)
                sum += (remainder + 1);
            else
            if (div == 1)
                sum += 2 * (remainder + 1);
            else
            {
                sum += (remainder + 1) * ( (number / key2 + 1) * func2(key, div / key));
                for (BigInteger i = 0; i <= number / key2 - 1; i++)
                {
                    if (i == 0)
                        sum += key * (remainder + 1) ;
                    else
                    if (i == 1)
                        sum += 2 * key * (remainder + 1);
                    else
                    {
                        sum += func2(key, i) * key * (remainder + 1);
                    }
                }
            }


            return sum ;

    }

    private BigInteger func(BigInteger key,BigInteger number)
        {

            BigInteger remainder;
            BigInteger div = BigInteger.DivRem(number, key, out remainder);
            BigInteger sum = 0;

            for (BigInteger i = 0; i <= div - 1; i++)
            {
                if (i == 0)
                    sum += key;
                else
                if (i == 1)
                    sum += 2 * key;
                else
                {
                    sum += func(key, i) * key ;
                }
            }

            if (div == 0)
                sum += (remainder + 1);
            else
            if (div == 1)
                sum += 2 * (remainder + 1);
            else
            {
                sum += func(key, div) * (remainder + 1);
            }


            return sum;
        }

        private void DoCalc(Tuple<IActor, BigInteger, BigInteger> msg)
        {
            BigInteger sum = func2(msg.Item2, msg.Item3);
            var ans = new Euler546Message()
            {
                Sender = this,
                Key = msg.Item2,
                Number = msg.Item3,
                Sum = sum
            };
            msg.Item1.SendMessage(ans);
        }


    }
}


