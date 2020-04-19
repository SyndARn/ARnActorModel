namespace Actor.Base
{
    /// <summary>
    /// How an actor is handled in running context
    /// </summary>
    public interface IRunningActor
    {
        BaseActor Actor { get; }
    }
}
