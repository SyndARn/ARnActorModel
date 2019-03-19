namespace Actor.Base
{
    public interface IActorMailBox<T>
    {
        bool IsEmpty { get; }

        void AddMessage(T aMessage);
        void AddMiss(T aMessage);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        T GetMessage();

        int RefreshFromMissed();
    }
}