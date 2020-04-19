namespace Actor.Base
{
    public class SupervisedBehavior : Behavior<SupervisorAction>
    {
        public SupervisedBehavior()
        {
            Pattern = t => t == SupervisorAction.Kill;
            Apply = _ => LinkedActor.SendMessage(SystemMessage.NullBehavior);
        }
    }
}
