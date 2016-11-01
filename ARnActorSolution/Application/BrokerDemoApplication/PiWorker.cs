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
    class PiWorker : WorkerActor<int>
    {
        CollectionActor<string> fMemLogger;

        public PiWorker(CollectionActor<string> memLogger) : base()
        {
            fMemLogger = memLogger;
        }

        protected override void Process(int value)
        {
            Random random = new Random(value);

            double inDisc = 0;
            for (int i = 0; i <= 1000; i++)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();
                if (x * x + y * y <= 1.0)
                {
                    inDisc++;
                }
            }
            double result = 4.0 * inDisc / 10000.0;
            fMemLogger.Add(string.Format("Value {0} Pi Approx {1}", value, result)) ;
        }
    }
}
