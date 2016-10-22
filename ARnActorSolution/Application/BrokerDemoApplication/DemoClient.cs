using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using Actor.Server;

namespace BrokerDemoApplication
{
    public class DemoClient
    {

        public DemoClient(BrokerActor<string> aBroker)
        {
            foreach(var item in Enumerable.Range(1, 10000))
                aBroker.SendMessage(String.Format("Start Job {0}",item));
        }
    }
}
