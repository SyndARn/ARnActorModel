using System;
using System.Diagnostics;
using Actor.Base;

/// <summary>
/// WinSkyNet1M
/// Compare ARnActor to other 
/// https://github.com/atemerev/skynet
/// </summary>
namespace WinSkyNet1M
{
    public class Skynet1mBenchmark
    {
        public Skynet1mBenchmark()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var future = new Future<long>();
            var skynet = new SkynetActor(null, 0, 1000000, 10, future);
            Console.WriteLine($"ms : {sw.Elapsed.TotalMilliseconds:0.000} - result : {future.ResultAsync().Result}");
            Console.ReadLine();
        }
    }

    public class SkynetActor : BaseActor
    {
        private readonly long _number;
        private readonly IActor _parent;
        private long _accumulator;
        private readonly IFuture<long> _future;
        private long _actorQtt;

        public SkynetActor(IActor parent, long number, long size, long dv, IFuture<long> future)
        {
            _number = number;
            _parent = parent;
            _future = future;
            _actorQtt = dv;
            Become(new Behavior<long>((i) =>
             {
                 _accumulator += i;
                 _actorQtt--;
                 if (_parent != null)
                 {
                     if (_actorQtt ==0)
                         _parent.SendMessage(_accumulator);
                 }
                 else
                 {
                     if ((future != null) && (_actorQtt == 0))
                         future.SendMessage(_accumulator);
                 }
             }));
            AddBehavior(new Behavior<string>((s) =>
            {
                if (size != 1)
                {
                    for (int i = 0; i < dv; i++)
                    {
                        var sub_num = _number + (i * (size / dv));
                        new SkynetActor(this, sub_num, size / dv, dv, null);
                    }
                }
                else
                {
                    _parent.SendMessage(_number);
                }
            })) ;
            SendMessage("start");
        }
    }
}
