using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace BenchmarkForActor
{
    public class Accumulator : BaseActor
    {
        private double fAccumulate = 0;

        public Accumulator()
        {
            Become(new Behavior<double>
                (
                i => fAccumulate++
                ));

            AddBehavior(new Behavior<IFuture<double>>
                (
                    f => f.SendMessage(fAccumulate)
                ));
        }
    }

    public class BenchActor : BaseActor
    {
        public BenchActor(IActor anActor)
        {
            Become(new Behavior<double>(
                i => true,
                i => anActor.SendMessage(i)
                ));
        }
    }
}
