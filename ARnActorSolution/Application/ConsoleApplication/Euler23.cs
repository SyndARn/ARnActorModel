using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace PrimeSumNumber_Euler543
{
    enum PerfectCategory { None, Perfect, Deficient, Abundant }

    class CategoryNumber : BaseActor
    {
        public CategoryNumber() : base()
        {
            Become(new Behavior<IActor, long>(DoCategory));
        }

        private void DoCategory(IActor target,long number)
        {
            // find divisors
            List<long> divisors = new List<long>();
            long limit = number / 2 + 1;
            for(long i =1; i < limit;i++)
            {
                if (number % i == 0)
                    divisors.Add(i);
            }
            // return number / divisor
            PerfectCategory cat = PerfectCategory.None;
            long sum = divisors.Count == 0 ? 0 : divisors.Sum();
            if (number == sum) cat = PerfectCategory.Perfect;
            else
            if (number >= sum) cat = PerfectCategory.Deficient;
            else cat = PerfectCategory.Abundant;
            target.SendMessage(new Tuple<long, long, PerfectCategory>(number, sum,cat));
        }
    }

    class AllCategoryNumberBelow : BaseActor
    {
        private List<Tuple<long, long, PerfectCategory>> list = new List<Tuple<long, long, PerfectCategory>>();
        private long entries;
        IActor caller;

        public AllCategoryNumberBelow() : base()
        {
            Become(new Behavior<IActor, long>(DoIt));
            AddBehavior(new Behavior<IActor>(CopyTo));
        }

        private void DoIt(IActor actor,long number)
        {
            caller = actor;
            Become(new Behavior<Tuple<long, long, PerfectCategory>>(Receive));
            for(long i = 1; i<= number;i++)
            {
                entries++;
                CategoryNumber cat = new CategoryNumber();
                cat.SendMessage(new Tuple<IActor, long>(this, i));
            }
        }

        private void Receive(Tuple<long, long, PerfectCategory> msg)
        {
            list.Add(msg);
            entries--;
            if (entries == 0)
            {
                CopyTo(caller);
            }
        }

        private void CopyTo(IActor msg)
        {
            msg.SendMessage(list.ToList());
        }
    }

    class Euler23 : BaseActor
    {
        public Euler23() : base()
        {

        }

        public List<Tuple<long, long, PerfectCategory>> Calc(long n)
        {
            var cn = new AllCategoryNumberBelow();
            cn.SendMessage(new Tuple<IActor,long>(this, n));
            var r = Receive(t => t is List<Tuple<long, long, PerfectCategory>>);
            return (List<Tuple<long, long, PerfectCategory>>)r.Result ;
        }
    }
}
