using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class RxObservable<T> : BaseActor, IObservable<T>
    {
        private List<IObserver<T>> observers;
        public RxObservable()
        {
            observers = new List<IObserver<T>>();
            var bhv1 = new Behavior<IObserver<T>>(DoSubscribe);
            var bhv2 = new Behavior<T>(DoTrack);

            Become(bhv1);
            AddBehavior(bhv2);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée",Justification = "IDispo is sent as message")]
        private void DoSubscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            IDisposable dispo = new Unsubscriber(observers, observer);
            SendMessage(new Tuple<IActor, IDisposable>(this, dispo));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Task<Object> res = Receive(t => { return t is Tuple<IActor, IDisposable>; });
            SendMessage(observer);
            var resi = res.Result as Tuple<IActor, IDisposable>;
            return resi.Item2;
        }

        public void Track(T loc)
        {
            SendMessage(loc);
        }

        private void DoTrack(T loc)
        {
            foreach (var observer in observers)
            {
                if (loc == null)
                    observer.OnError(new Exception());
                else
                    observer.OnNext(loc);
            }
        }

        private void DoEndTransmission(T observer)
        {
            foreach (var item in observers.ToArray())
                if (observers.Contains(item))
                    item.OnCompleted();

            observers.Clear();
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}