using Actor.Base;

namespace Actor.Util
{
    public class RealActor : BaseActor
    {
        private string fData;
        public RealActor() : base()
        {
            var behaviorStore = new Behavior<string>(t => fData = t);
            var behaviorRetrieve = new Behavior<IFuture<string>>((IFuture<string> t) => t.SendMessage(fData));
            Become(behaviorStore, behaviorRetrieve);
        }
    }
}
