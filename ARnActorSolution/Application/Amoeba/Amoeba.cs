using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.MonteCarlo;
using Actor.Base;
using System.Windows.Forms;

namespace Amoeba
{
    public class ResultActor : BaseActor
    {
        private List<long> results = new List<long>();
        public ResultActor()
        {
            Become(new Behavior<long>(ResultMessage));
            AddBehavior(new Behavior<IActor>(FutureMessage));
        }

        private void ResultMessage(long s)
        {
            results.Add(s);
        }

        private void FutureMessage(IActor aFuture)
        {
            long total = results.Count(r => r <= 0);
            long qtt = results.Count();
            aFuture.SendMessage(total, qtt);
        }

        public Future<long,long> GetResult()
        {
            Future<long, long> future = new Future<long, long>();
            SendMessage(future);
            return future;
        }
    }

    // main class actor
    public class AmoebaActor
    {
        MonteCarloActor<long> fMonteCarlo = new MonteCarloActor<long>();

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
