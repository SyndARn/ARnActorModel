using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public enum ObservableAction { Register, Unregister} ;

    public class actObservable<T> : actActor
    {
        private actCollection<IActor> fCollection;

        public actObservable() : base()
        {
            fCollection = new actCollection<IActor>();
            Become(new bhvBehavior<string>(DoStart));
            SendMessageTo("Start Observe");
        }

        public void PublishData(T aT)
        {
            SendMessageTo(aT);
        }

        private void DoStart(string msg)
        {
            Become(new bhvBehavior<Tuple<ObservableAction,IActor>>(DoRegister)) ;
            AddBehavior(new bhvBehavior<T>(DoPublishData)) ;
        }

        private void DoRegister(Tuple<ObservableAction,IActor> msg)
        {
            if (msg.Item1.Equals(ObservableAction.Register))
            {
                fCollection.Add(msg.Item2);
            } else
            {
                fCollection.Remove(msg.Item2);
            }
        }

        private void DoPublishData(T aT)
        {
            var bct = new actBroadCast<T>();
            bct.BroadCast(aT, fCollection);
        }

    }
}
