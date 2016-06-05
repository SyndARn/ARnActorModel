using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Bench
{
    class Program
    {
        static void Main(string[] args)
        {
            var run = new Runner();
            var max = BigInteger.Pow(10, 7);
            var bench = new Bencher(max);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            run.Run(bench, max);
            stopwatch.Stop();
            Console.WriteLine(bench.GetResultAsync().Result+" "+stopwatch.Elapsed);
            Console.ReadLine();
        }
    }
}
