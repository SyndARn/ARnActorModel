using Actor.Base;

namespace Actor.Util
{
    public class CrudActor<TKey, TValue> : BaseActor
    {
        public CrudActor()
            : base() => Become(new CrudBehavior<TKey, TValue>());

        public Future<TValue> Get(TKey key)
        {
            Future<TValue> future = new Future<TValue>();
            SendMessage(new CrudMessage<TKey, TValue>(CrudAction.Get, key, default, (IActor)future));
            return future;
        }

        public void Set(TKey key, TValue value) => SendMessage(new CrudMessage<TKey, TValue>(CrudAction.Set, key, value, null));

        public void Delete(TKey key) => SendMessage(new CrudMessage<TKey, TValue>(CrudAction.Delete, key, default, null));

        public void Update(TKey key, TValue value) => SendMessage(new CrudMessage<TKey, TValue>(CrudAction.Update, key, value, null));
    }
}
