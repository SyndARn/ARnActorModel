using Actor.Base;
using Actor.Server;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.MonteCarlo
{
    /*
     * pricer
     * shard list
     * scheduler send to shard
     * await result
     */


    public class ActorScheduler : ActionActor
    {
        static IActor fActor;
        ShardList shardList;
        /* find pricer on shard */
        /* send pricer */
        /* await result */
        public ActorScheduler() : base()
        {
            fActor = this;
            shardList = new ShardList();
            Become(new Behavior<String>(DoScheduling));
            AddBehavior(new Behavior<double>(DoCollectData));
            this.SendMessage("Start");
        }

        private void DoCollectData(double data)
        {
            // actSendByName<string>.SendByName(string.Format("receive {0}",data), "Console");
            shardList.Add(data.ToString(CultureInfo.InvariantCulture));
            var ct = shardList.Count();
            Console.WriteLine("receive "+ct);
            if (ct >= 1000-1)
                SendByName<string>.Send("Done", "Console");
        }

        private void DoScheduling(string msg)
        {
            // shard list
            shardList = new ShardList();

            for (int i = 0; i < 1000; i++)
            {
                var act = new ActorPrice();
                List<double> list = new List<double>
                {
                    i
                };
                act.ConsoleWrite((IActor)this, list.AsEnumerable());
            }
        }
    }

    public class ShardList : CollectionActor<string>
    {
    }



    public class ActorPrice : ActionActor<IActor,IEnumerable<double>>
    {
        public void ConsoleWrite(IActor actor, IEnumerable<double> someDoubles)
        {
            SendAction(DoConsoleWrite, actor, someDoubles);
        }

        private void DoConsoleWrite(IActor actor, IEnumerable<double> someDoubles)
        {
            double accumulator = 0 ;
            foreach(var dbl in someDoubles)
            {
                accumulator = Math.Sin(dbl);
            }
            // Console.WriteLine("action receiver " + accumulator);
            actor.SendMessage(accumulator);
        }
    }
}
