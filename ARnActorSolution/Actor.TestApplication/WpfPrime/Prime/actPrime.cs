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
            AddBehavior(new bhvBehavior<string>(Start));
        }

        const int slice = 20;

        private void Start(string msg)
        {
            List<IActor> aList = new List<IActor>();
            for (int i = 0; i < slice; i++)
            {
                aList.Add(new actPrime(prime));
            }

            Parallel.For(0, slice,
                f =>
                {
                    var act = aList[f];
                    for (int i = prime / slice; i > 0 ; i--)
                    {
                        act.SendMessage(new Tuple<int, IActor>(
                            prime/slice*f + i
                            , this));
                    }
                }
                );

        }

        private void FindPrime(int msg)
        {
            Console.WriteLine("Find diviser {0} of {1}", msg, prime);
        }

        private void DoPrime(Tuple<int, IActor> msg)
        {
            if (msg.Item1 > 0)
            {
                if (prime % msg.Item1 == 0)
                {
                    msg.Item2.SendMessage(msg.Item1);
                }
            }
        }
    }
}
