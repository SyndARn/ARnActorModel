using Actor.Base;
using System.Threading.Tasks;

namespace Actor.Server
{
    public class HeartBeatActor : BaseActor
    {
        private int fTimeOutMs;
        public HeartBeatActor(int timeOutMs)
        {
            fTimeOutMs = timeOutMs;
            Become(new Behavior<IActor>(a =>
            {
                a.SendMessage(this);
                Task.Delay(fTimeOutMs).Wait();
                SendMessage(a);
            }));
        }
    }
}
