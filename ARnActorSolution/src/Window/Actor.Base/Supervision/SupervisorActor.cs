namespace Actor.Base
{

    public class SupervisorActor : BaseActor
    {
        public SupervisorActor() : base (new SupervisorBehavior())
        {

        }
    }
}
