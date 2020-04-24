using Actor.Base;
using Actor.MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amoeba
{
    class PiActor : BaseActor
    {
        readonly List<Tuple<double, double>> results = new List<Tuple<double, double>>();

        public PiActor()
        {
            Become(new Behavior<double, double>((q, s) =>
             {
                 results.Add(new Tuple<double, double>(q, s));
             }));
            AddBehavior(new Behavior<IActor>(GetResult));
        }

        private void GetResult(IActor actor)
        {
            double prob = 4.0*results.Sum(t => t.Item2) / results.Sum(t => t.Item1); // it approximates pi/4
            actor.SendMessage(prob.ToString());
        }

        public async Task<string> GetResultAsync()
        {
            var future = new Future<string>();
            SendMessage(future);
            return await future.ResultAsync();
        }

        public void Launch(long nbSimul)
        {
            MonteCarloActor<double>.Cast((simnum, itera, result) =>
            {
                if (itera <= 0)
                {
                    return;
                }
                Random random = new Random((int)simnum);

                double inDisc = 0;
                for (int i = 0; i <= itera; i++)
                {
                    var x = random.NextDouble();
                    var y = random.NextDouble();
                    if (x * x + y * y <= 1.0)
                    {
                        inDisc++;
                    }
                }
                
                result.SendMessage(itera,inDisc);
            }, 100000, this, nbSimul);
        }
    }
}
