using Actor.Base;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.MonteCarlo
{
/*
 * random source
 * qtt of simul
 * simul as Action
 * result collector
 */

    public class MonteCarloActor<TInput> : BaseActor
    {
        public MonteCarloActor()
        {
            Become(new Behavior<Action<TInput,IActor>,TInput, IActor, int>(Process));
        }

        public static MonteCarloActor<TInput> Cast(
            Action<TInput,IActor> simulation, 
            TInput data,
            IActor result, 
            int simulationQtt)
        {
            var r = new MonteCarloActor<TInput>();
            r.SendMessage(simulation, data, result, simulationQtt);
            return r;
        }

        private void Process(Action<TInput,IActor> simulation, TInput data, IActor result, int simulationQtt)
        {
            for(int i =0;i<simulationQtt;i++)
            {
                var actor = new BaseActor(new Behavior<Action<TInput, IActor>, TInput, IActor>(
                    (action, input, act) => action(input, act)));
                actor.SendMessage(simulation, data, result);
            }
        }
    }

}
