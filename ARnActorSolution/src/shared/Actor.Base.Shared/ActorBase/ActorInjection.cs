namespace Actor.Base
{
    /// <summary>
    /// How an actor is handled in running context
    /// </summary>
    public interface IRunningActor
    {
        BaseActor Actor { get; }
    }

    /// <summary>
    /// How to inject actor into another object
    /// Use Behaviours and and a public interface around
    /// </summary>

    public class ActorInjection : IRunningActor
    {
        public static ActorInjection Cast(IBehaviors bhvs)
        {
            return new ActorInjection
            {
                Actor = new BaseActor(bhvs)
            };
        }

        public BaseActor Actor { get; private set; }

        // now you can injection.actor.sendmessageto ...
    }
}
