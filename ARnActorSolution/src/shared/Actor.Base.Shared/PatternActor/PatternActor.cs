namespace Actor.Base
{
    public abstract class PatternActor<T> : BaseActor
    {
        protected PatternActor() : base()
        {
            Become(new Behavior<T>(DoApply));
        }

        protected abstract void DoApply(T msg);
    }
}
