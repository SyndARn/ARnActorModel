using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using Actor.Server;

namespace WpfPrime.Prime
{
    public class actPrime : actActor
    {
        int prime;
        public actPrime(int n)
        {
            prime = n;
            Become(new bhvBehavior<Tuple<int, IActor>>(DoPrime));
            AddBehavior(new bhvBehavior<int>(FindPrime));
        }


        private void FindPrime(int msg)
        {
            Console.WriteLine("Find diviser %0 of %1", msg, prime);
        }

        private void DoPrime(Tuple<int, IActor> msg)
        {
            if (msg.Item1 > 1)
            {
                if (msg.Item1 % prime == 0)
                {
                    msg.Item2.SendMessage(prime);
                }
                var actn1 = new actPrime(msg.Item1 -1);
                for (int i = msg.Item1 - 1; i > 0; i--)
                {
                    actn1.SendMessage(new Tuple<int,IActor>(i, this));
                }
            }
        }
    }
}
