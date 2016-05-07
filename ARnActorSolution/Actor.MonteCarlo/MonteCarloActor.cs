using Actor.Base;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.MonteCarlo
{

    public class MonteCarloActor<TInput> : BaseActor
    {
        public MonteCarloActor()
        {
            Become(new Behavior<Action<long,TInput,IActor>,TInput, IActor, long>(Process));
        }

        public static MonteCarloActor<TInput> Cast(
            Action<long, TInput,IActor> simulation, 
            TInput data,
            IActor result, 
            long simulationQtt)
        {
            var r = new MonteCarloActor<TInput>();
            r.SendMessage(simulation, data, result, simulationQtt);
            return r;
        }

        private void Process(Action<long,TInput,IActor> simulation, TInput data, IActor result, long simulationQtt)
        {
            for(long i =0;i<simulationQtt;i++)
            {
                var actor = new BaseActor(new Behavior<Action<long, TInput, IActor>, TInput, long, IActor>(
                    (action, input, num, act) => action(num, input, act)));
                actor.SendMessage(simulation, data, i,result);
            }
        }
    }

}
