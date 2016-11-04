using Actor.Base;
using System.Threading.Tasks;

namespace Actor.Server
{
    public enum HeartBeatAction { Beat }

    public class HeartBeatActor : BaseActor
    {
        private int fTimeOutMS;
        public HeartBeatActor(int timeOutMS)
        {
            fTimeOutMS = timeOutMS;
            Become(new Behavior<IActor>((a) =>
            {
                a.SendMessage(this, HeartBeatAction.Beat);
                Task.Delay(fTimeOutMS).Wait();
                SendMessage(a);
            }));
        }
    }
}
