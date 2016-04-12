using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Util;

namespace PrimeSumNumber_Euler543
{
    public class Euler_7
    {
    }

    public class FindPrimeBelow : BaseActor
    {
        private HashSet<long> Primes = new HashSet<long>();

        public FindPrimeBelow()
        {
            Primes.Add(2);
            Primes.Add(3);
            Primes.Add(5);
            Primes.Add(7);
            Primes.Add(11);
            Primes.Add(13);
            Become(new Behavior<Tuple<IActor, long>>(DoFindPrime));
        }

        private bool CalcPrime(long n)
        {
            if (n <= 1) return false;
            if (n % 2 == 0) return false;
            if (n == 2) return true;
            if (n == 3) return true;
            if (Primes.Contains(n)) return true;

            double square = Math.Sqrt(n);
            for (long i = 3; i <= square; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            Primes.Add(n);
            return true;
        }

        private void DoFindPrime(Tuple<IActor, long> msg)
        {
            IActor caller = msg.Item1;
            long n = msg.Item2;
            for(long i = Primes.Last()+2; i <= n;i++)
                CalcPrime(i);
            caller.SendMessage(new Tuple<IActor, IEnumerable<long>>(this, Primes));
        }
    }

    public class FindAllPrime : BaseActor
    {
        private HashSet<long> Primes = new HashSet<long>();

        public FindAllPrime()
        {
            Primes.Add(2);
            Primes.Add(3);
            Primes.Add(5);
            Primes.Add(7);
            Primes.Add(11);
            Primes.Add(13);
            Become(new Behavior<Tuple<IActor, long>>(DoFindPrime));
        }

        private bool CalcPrime(long n)
        {
            if (n <= 1) return false;
            if (n % 2 == 0) return false;
            if (n == 2) return true;
            if (n == 3) return true;
            if (Primes.Contains(n)) return true;

            double square = Math.Sqrt(n);
            for (long i = 3; i <= square; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            Primes.Add(n);
            return true;
        }

        private void DoFindPrime(Tuple<IActor, long> msg)
        {
            IActor caller = msg.Item1;
            long n = msg.Item2;
            long i = 2;
            while (Primes.Count < n)
            {
                CalcPrime(i);
                i++;
            }
            caller.SendMessage(new Tuple<IActor, IEnumerable<long>>(this, Primes));
        }
    }

    public class FindPrime : BaseActor
    {
        private HashSet<long> Primes = new HashSet<long>();

        public FindPrime()
        {
            Primes.Add(2);
            Primes.Add(3);
            Primes.Add(5);
            Primes.Add(7);
            Primes.Add(11);
            Primes.Add(13);
            Become(new Behavior<Tuple<IActor,long>>(DoFindPrime));
        }

        private bool CalcPrime(long n)
        {
            if (n <= 1) return false;
            if (n % 2 == 0) return false;
            if (n == 2) return true;
            if (n == 3) return true;
            if (Primes.Contains(n)) return true;

            double square = Math.Sqrt(n);
            for(long i = 3; i <= square; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            Primes.Add(n);
            return true ;
        }

        private void DoFindPrime(Tuple<IActor, long> msg)
        {
            IActor caller = msg.Item1;
            long n = msg.Item2;
            bool result = CalcPrime(n);
            caller.SendMessage(new Tuple<IActor, bool>(this, result));
        }
    }
}
