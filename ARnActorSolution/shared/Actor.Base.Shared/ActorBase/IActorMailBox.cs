namespace Actor.Base
{
    public interface IActorMailBox<T>
    {
        bool IsEmpty { get; }

        void AddMessage(T aMessage);
        void AddMiss(T aMessage);
        T GetMessage();
        int RefreshFromMissed();
    }
}