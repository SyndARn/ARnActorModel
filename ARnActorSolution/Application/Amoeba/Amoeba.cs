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
            long total = results.Count(r => r > 0);
            long qtt = results.Count();
            aFuture.SendMessage(new Tuple<long,long>(total, qtt));
        }

        public Future<Tuple<long,long>> GetResult()
        {
            Future<Tuple<long, long>> future = new Future<Tuple<long, long>>();
            SendMessage((IActor)future);
            return future;
        }
    }

    public class Amoeba
    {
        MonteCarloActor<int> fMonteCarlo = new MonteCarloActor<int>();

        public Amoeba()
        {
        }

        public void Launch(ResultActor resultActor)
        {
            MonteCarloActor<long>.Cast((iteration, result) =>
            {
                if (iteration <= 0)
                {
                    return;
                }
                Random random = new Random();
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
            }, 30, resultActor, 1000000);
        }
    }
}
