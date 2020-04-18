namespace Actor.Base
{
    public class ActionBehaviors<T> : Behaviors
    {
        public ActionBehaviors()
        {
            AddBehavior(new ActionBehavior());
            AddBehavior(new ActionBehavior<T>());
        }
    }

    public class ActionBehaviors<T1, T2> : Behaviors
    {
        public ActionBehaviors()
        {
            AddBehavior(new ActionBehavior());
            AddBehavior(new ActionBehavior<T1, T2>());
        }
    }
}
