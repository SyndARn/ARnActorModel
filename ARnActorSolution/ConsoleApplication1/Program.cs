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
        static void Calc(BigInteger K, BigInteger N)
        {
                var act = new ReceiveActor<BigInteger>();
                var Euler546_3 = new Euler546_3(K);
                Console.WriteLine(string.Format("K {0} N {1} = {2}",K,N, act.Wait(Euler546_3, N).Result.Item2));
        }

        static void Main(string[] args)
        {
            ActorServer.Start("localhost", 1123, false);

            var prime = new FindPrimeBelowActor();
            var act = new ReceiveActor<long, IEnumerable<long>>();
            long qttprime = 1000000;
            var r = act.Wait(prime, qttprime);
            Console.WriteLine(string.Format("Primes below {0} : ", qttprime));
            //foreach(var item in r.Result.Item2.OrderBy(t => t))
            //{
            //    Console.WriteLine(item);
            //}
            long sum = r.Result.Item2.Sum(t => t);
            Console.WriteLine("Sum all of primes {0}", sum);

            // 131
            //foreach(var item in r.Result.Item2)
            //{
            //    for(long i = 1; i <= )
            //}


            // Calc(5, 10);
            //for (BigInteger i = 0; i<= 1000;i++)
            //    Calc(2, i);
            // Calc(5,10) ;
            // Calc(7, 100);
            // Calc(2, 1000);
            // Calc(10, 1000000);
            // Calc(10, BigInteger.Pow(10,14));


            //Euler546_2 = new Euler546_2(10);
            //r = act.Calc(Euler546_2, 100000000000000);
            //Console.WriteLine(r.Result.Item2);

            Console.ReadLine();
        }
    }
}
