using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using Actor.Server;
using System.Diagnostics;
using System.Globalization;

namespace BrokerDemoApplication
{
    public class DemoWorker : WorkerActor<string>
    {
        CollectionActor<string> fMemLogger;
        public DemoWorker(CollectionActor<string> memLogger) : base()
        {
            fMemLogger = memLogger;
        }
        protected override void Process(string aT)
        {
            StringBuilder sb = new StringBuilder();
            var rnd = new Random();
            foreach (var item in Enumerable.Range(1, 100))
            {
                char a = (char)rnd.Next(256);
                sb.Append(a);
            }
            var listStrings = sb.ToString().Select(t => t).OrderBy(t => t) ;
            fMemLogger.Add(
                string.Format(CultureInfo.InvariantCulture,"Ticks {0} Count {1} String {2}", 
                    System.Environment.TickCount,
                    listStrings.Count(), 
                    new string(listStrings.ToArray())));
            Task.Delay(rnd.Next(100)).Wait();
        }
    }
}
