using Actor.Base;

namespace Actor.Util
{
    public class ActorProxyModel<T> : BaseActor
        where T : class
    {
        private T _t;

        public ActorProxyModel() : base()
        {
        }

        internal void SetObject(T aT) => _t = aT;

        internal void AddBehaviors(Behaviors behaviors) => Become(behaviors);

        public void SendMethodAndParam(string methodName, object param) => this.SendMessage(methodName, param);
    }
}
