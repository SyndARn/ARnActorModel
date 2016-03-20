using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util ;

namespace Actor.TestApplication
{
    public class actMillion : actActor
    {
        private actQueue<IActor> fQueue;
        const int KSize = 10000 ;
        public actMillion() : base()
        {
            fQueue = new actQueue<IActor>();
            Become(new bhvBehavior<string>(DoStart));
            SendMessage("DoStart");
        }

        public void Send()
        {
            Become(new bhvBehavior<string>(DoSend));
            SendMessage("DoSend");
        }

        private void DoStart(string msg)
        {
            for (int i = 0; i < KSize; i++)
            {
                fQueue.Queue(new actActor());
            }
            Console.WriteLine("end start million");
        }

        private void DoSend(string msg)
        {
            int i = 0;
            msgQueue<IActor> item = fQueue.TryDequeue().Result;
            while(item.Result && (i<KSize))
            {
                item.Data.SendMessage("Bop");
                item = fQueue.TryDequeue().Result;
                Console.WriteLine("receive " + i.ToString());
                i++;
            }
            Console.WriteLine("end "+i.ToString());
        }
    }
}
