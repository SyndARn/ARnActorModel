using Actor.Base;

namespace Actor.Util
{
    public class CrudMessage<TKey, TValue>
    {
        public CrudMessage(CrudAction anAction, TKey aKey, TValue aValue, IActor sender)
        {
            Action = anAction;
            Key = aKey;
            Value = aValue;
            Sender = sender;
        }

        public CrudAction Action { get; }
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public IActor Sender { get; set; }
    }
}
