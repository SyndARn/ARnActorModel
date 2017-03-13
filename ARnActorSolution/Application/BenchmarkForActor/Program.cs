using Actor.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace BenchmarkForActor
{
    class Program
    {
        static void BenchProc()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Accumulator accumulator = new Accumulator();
            IFuture<double> future = new Future<double>();
            List<IActor> actorList = new List<IActor>();
            for (int i = 0; i < 1000; i++)
                actorList.Add(new BenchActor(accumulator));
            double lastElapsed = 0;
            int twenysecbench = 100;
            while (twenysecbench >= 0)
            {
                for (int i = 0; i < 1000; i++)
                {
                    actorList[i].SendMessage(1.0);
                }
                if (stopwatch.ElapsedMilliseconds > lastElapsed + 1000 )
                {
                    lastElapsed = stopwatch.ElapsedMilliseconds;
                    accumulator.SendMessage(future);
                    double speed = future.Result() / lastElapsed *1000.0;
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Speed {0}", speed));
                    twenysecbench--;
                }
            }
        }

        static void Main(string[] args)
        {
            BenchProc();
            Console.ReadLine();
        }
    }
}
