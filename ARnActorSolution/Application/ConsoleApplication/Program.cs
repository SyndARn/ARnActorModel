using Actor.Base;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Actor.Util;

namespace PrimeSumNumber_Euler543
{
    class Program
    {
        static void Calc546(BigInteger K, BigInteger N)
        {
            var Euler546 = new Euler546_3();
            var rec = new Receiver();
            var r = rec.Call(Euler546,K, N);
            Console.WriteLine(string.Format("K {0} N {1} = {2}",K,N,r.Sum));
        }

        static void Calc23(long i)
        {
            var euler23 = new Euler23();
            var r = euler23.Calc(i);

            foreach (var item in r)
            {
                Console.WriteLine(string.Format("Integer {0} Sum {1} Category {2}", item.Item1, item.Item2, item.Item3));
            }

            var abundants =
                r.Where(t => t.Item3 == PerfectCategory.Abundant).Select(t => t.Item1).OrderBy(t => t);

            List<long> abundantSum = new List<long>();

            for(long k = abundants.First();k<=28123;k++)
            {
                bool abund = false;
                foreach(var item1 in abundants.Where(t => t <= k))
                {
                    var item2 = abundants.FirstOrDefault(t => t == k - item1);
                    if (item2 == 0)
                        break;
                        if (k < (item1 + item2))
                            break;
                        if (k == item1+item2)
                        {
                        abund = true; 
                            break;
                        }
                }
                if (!abund)
                {
                    abundantSum.Add(k);
                    Console.WriteLine("Find {0}", k);
                }
            }

        }

        static void Main(string[] args)
        {
            ActorServer.Start("localhost", 1123, null);
            // Euler 23
            // Calc23(28123);
            
            // var prime = new FindPrimeBelowActor();
            // var act = new ReceiveActor<long, IEnumerable<long>>();
            // long qttprime = 1000000;
            // var r = act.Wait(prime, qttprime);
            // Console.WriteLine(string.Format("Primes below {0} : ", qttprime));
            // long sum = r.Result.Item2.Sum(t => t);
            // Console.WriteLine("Sum all of primes {0}", sum);
            //foreach(var item in r.Result.Item2.OrderBy(t => t))
            //{
            //    Console.WriteLine(item);
            //}

            // 131
            //foreach(var item in r.Result.Item2)
            //{
            //    for(long i = 1; i <= )
            //}


            // Euler 546
            // Calc546(5,10) ;
            // Calc546(7, 100);
            Calc546(2, 1000);
            Calc546(10, 1000000);
            // Calc(10, BigInteger.Pow(10,14));


            Console.ReadLine();
        }
    }
}
