using Actor.Base;
using Actor.Util ;
using System;
using System.Collections.Generic;
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


    public class actScheduler : actActionActor
    {
        static IActor fActor;
        ShardList shardList;
        /* find pricer on shard */
        /* send pricer */
        /* await result */
        public actScheduler() : base()
        {
            fActor = this;
            shardList = new ShardList();
            Become(new bhvBehavior<String>(DoScheduling));
            AddBehavior(new bhvBehavior<double>(DoCollectData));
            SendMessage("Start");
        }

        private void DoCollectData(double data)
        {
            // actSendByName<string>.SendByName(string.Format("receive {0}",data), "Console");
            shardList.Add(data.ToString());
            var ct = shardList.Count();
            Console.WriteLine("receive "+ct);
            if (ct >= 1000-1)
                actSendByName<string>.SendByName("Done", "Console");
        }

        private void DoScheduling(string msg)
        {
            // shard list
            shardList = new ShardList();

            for (int i = 0; i < 1000; i++)
            {
                var act = new actPrice();
                List<double> list = new List<double>();
                list.Add(i);
                act.ConsoleWrite(Tuple.Create((IActor)this, list.AsEnumerable()));
            }
        }
    }

    public class ShardList : actCollection<string>
    {
    }



    public class actPrice : actAction<Tuple<IActor,IEnumerable<double>>>
    {
        public void ConsoleWrite(Tuple<IActor,IEnumerable<double>> someDoubles)
        {
            SendAction(DoConsoleWrite, someDoubles);
        }

        private void DoConsoleWrite(Tuple<IActor,IEnumerable<double>> someDoubles)
        {
            double accumulator = 0 ;
            foreach(var dbl in someDoubles.Item2)
            {
                accumulator = Math.Sin(dbl);
            }
            // Console.WriteLine("action receiver " + accumulator);
            someDoubles.Item1.SendMessage(accumulator);
        }
    }
}
