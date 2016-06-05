using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using System.Numerics;

namespace Bench
{
    public class Bencher : BaseActor
    {
        private BigInteger fTotal = 0;
        private BigInteger fCount = 0;
        private Future<string> future = new Future<string>();
        public Bencher(BigInteger max)
        {
            Become(new Behavior<string>(s =>
            {
                BigInteger l;
                if (BigInteger.TryParse(s,out l))
                {
                    fTotal += l;
                    fCount++;
                    if (fCount >= max)
                        future.SendMessage(string.Format("Total {0}", fTotal));
                }
            }));
        }
        public async Task<string> GetResultAsync()
        {
            return await future.ResultAsync();
        }
    }

    public class Worker : BaseActor
    {
        public Worker(IActor aBencher)
        {
            Become(new Behavior<BigInteger>(l =>
            {
                aBencher.SendMessage(l.ToString());
            }
            ));
        }
    }

    public class Runner
    {
        static int th = 8;
        public void Run(IActor bencher, BigInteger max)
        {
            Worker[] workerList = new Worker[th];
            for (int i = 0; i < th; i++)
                workerList[i] = new Worker(bencher);
            for(BigInteger i = 0; i<max; i++)
            {
                int m = (int)BigInteger.Remainder(i, th);
                var act = workerList[m];
                act.SendMessage(i);
            }
        }
    }

}
