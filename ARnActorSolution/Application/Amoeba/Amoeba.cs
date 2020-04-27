using System;
using System.Collections.Generic;
using System.Linq;
using Actor.MonteCarlo;
using Actor.Base;

namespace Amoeba
{
    public class ResultActor : BaseActor
    {
        private readonly List<long> _results = new List<long>();
        public ResultActor()
        {
            Become(new Behavior<long>(ResultMessage));
            AddBehavior(new Behavior<IActor>(FutureMessage));
        }

        private void ResultMessage(long s)
        {
            _results.Add(s);
        }

        private void FutureMessage(IActor aFuture)
        {
            long total = _results.Count(r => r <= 0);
            long qtt = _results.Count();
            aFuture.SendMessage(total, qtt);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Future<long,long> GetResult()
        {
            Future<long, long> future = new Future<long, long>();
            SendMessage(future);
            return future;
        }
    }

    public class AmoebaActor
    {
        readonly MonteCarloActor<long> fMonteCarlo = new MonteCarloActor<long>();

        public AmoebaActor()
        {
        }

        public void Launch(IActor resultActor)
        {
            MonteCarloActor<long>.Cast((simnum, iteration, result) =>
            {
                if (iteration <= 0)
                {
                    return;
                }
                Random random = new Random((int)simnum);
                long population = 1;
                for (long i = 0; i < iteration; i++)
                {
                    long newPopulation = 0;

                        for (long p = 0; p < population; p++)
                        {
                            newPopulation += random.Next(4);
                        }
                    population = newPopulation;
                    if (population <= 0)
                        break;
                }
                result.SendMessage(population);
            }, 30, resultActor, 100000);
        }
    }
}
