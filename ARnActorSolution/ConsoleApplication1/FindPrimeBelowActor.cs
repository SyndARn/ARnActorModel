using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace PrimeSumNumber_Euler543
{
    public class FindPrimeBelowActor : BaseActor
    {
        private HashSet<long> Primes = new HashSet<long>();

        private IActor Caller;

        private long Entries;

        private long N;

        public FindPrimeBelowActor()
        {
            Primes.Add(2);
            Primes.Add(3);
            Primes.Add(5);
            Primes.Add(7);
            Primes.Add(11);
            Primes.Add(13);
            Become(new Behavior<Tuple<IActor, long>>(DoFindPrime));
        }

        private void WaitForAPrime(Tuple<IActor, long, bool> msg)
        {
            if (msg.Item3)
                Primes.Add(msg.Item2);
            Entries--;
            if (Entries == 0)
                Caller.SendMessage(new Tuple<IActor, IEnumerable<long>>(this, Primes));
        }

        private void DoFindPrime(Tuple<IActor, long> msg)
        {
            Caller = msg.Item1;
            N = msg.Item2;
            Entries = 0;
            Become(new Behavior<Tuple<IActor, long, bool>>(WaitForAPrime));
            if (N >= Primes.Last() + 2)
            {
                for (long i = Primes.Last() + 2; i <= N; i += 2)
                {
                    Entries++;
                    var act = new CalcPrimeActor();
                    act.SendMessage(new Tuple<IActor, long>(this, i));
                }
            }
            else
            {
                Caller.SendMessage(new Tuple<IActor, IEnumerable<long>>(this, Primes));
            }
        }
    }

    public class CalcPrimeActor : BaseActor
        {
        public CalcPrimeActor() : base()
        {
            Become(new Behavior<Tuple<IActor, long>>(DoCalcPrime));
        }

        private bool CalcPrime(long n)
        {
            if (n <= 1) return false;
            if (n % 2 == 0) return false;
            if (n == 3) return true;

            double square = Math.Sqrt(n);
            for (long i = 3; i <= square; i += 2)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void DoCalcPrime(Tuple<IActor,long> msg)
        {
            IActor caller = msg.Item1;
            long n = msg.Item2;
            bool result = CalcPrime(n);
            caller.SendMessage(new Tuple<IActor, long, bool>(this, n, result));
        }
    }
}
