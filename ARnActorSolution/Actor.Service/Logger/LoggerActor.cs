using Actor.Server;
using Actor.Base;

namespace Actor.Service
{
    public class LoggerActor : BaseActor
    {
        public LoggerActor(string aFileName) : base()
        {
            Become(new LoggerBehaviors(aFileName));
            var heartBeat = new HeartBeatActor(500);
            heartBeat.SendMessage(this);
            SendMessage("Logging start");
        }
    }
}
