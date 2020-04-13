using Actor.Base;

namespace Actor.Server
{
    public class EchoActor<T> : BaseActor
    {
        public EchoActor(IActor dest, T aT)
        {
            CheckArg.Actor(dest);
            Become(new ConsoleBehavior<string>());
            dest.SendMessage(this, aT);
        }
    }

    public class EchoActor : BaseActor
    {
        public EchoActor(IActor dest, string value)
        {
            CheckArg.Actor(dest);
            Become(new ConsoleBehavior());
            dest.SendMessage(this,value);
        }
    }
}
