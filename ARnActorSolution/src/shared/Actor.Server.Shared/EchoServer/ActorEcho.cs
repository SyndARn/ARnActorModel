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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1806:Ne pas ignorer les résultats des méthodes", Justification = "<En attente>")]
        public static void Echo(IActor dest, string value)
        {
            new EchoActor(dest, value);
        }
    }
}
