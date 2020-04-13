using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class RXObservable<T> : BaseActor, IObservable<T>
    {
        private readonly List<IObserver<T>> _observers;

        public RXObservable()
        {
            _observers = new List<IObserver<T>>();
            Become(new Behavior<IObserver<T>>(DoSubscribe));
            AddBehavior(new Behavior<T>(DoTrack));
        }

        private void DoSubscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            IDisposable dispo = new Unsubscriber(_observers, observer);
            this.SendMessage(this, dispo);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Task<object> res = ReceiveAsync(t => t is IMessageParam<IActor, IDisposable>);
            SendMessage(observer);
            IMessageParam<IActor, IDisposable> resi = res.Result as IMessageParam<IActor, IDisposable>;
            return resi.Item2;
        }

        public void Track(T loc) => SendMessage(loc);

        private void DoTrack(T loc)
        {
            foreach (IObserver<T> observer in _observers)
            {
                if (loc == null)
                {
                    observer.OnError(new ActorException());
                }
                else
                {
                    observer.OnNext(loc);
                }
            }
        }

        private void DoEndTransmission(T observer)
        {
            foreach (IObserver<T> item in _observers.ToArray())
            {
                if (_observers.Contains(item))
                {
                    item.OnCompleted();
                }
            }

            _observers.Clear();
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer == null || !_observers.Contains(_observer))
                {
                    return;
                }

                _observers.Remove(_observer);
            }
        }
    }
}