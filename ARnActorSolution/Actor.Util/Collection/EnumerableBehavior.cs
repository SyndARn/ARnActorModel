using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace Actor.Util
{

    public class EnumerableActor<T> : BaseActor, IEnumerable<T>, IEnumerable, ICollection<T>
    {
        private List<T> fList = new List<T>();

        public int Count
        {
            get
            {
                var future = new Future<int>();
                this.SendMessage<Action<IActor>, IActor>((a) =>
                {
                    a.SendMessage(fList.Count);
                }, future);
                return future.Result();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
                //var future = new Future<bool>();
                //this.SendMessage<Action<IActor>, IActor>((a) =>
                //{
                //    a.SendMessage(false);
                //}, future);
                //return future.Result();
            }
        }

        public EnumerableActor() : base()
        {
            Become(new Behavior<Action<T>, T>((a, t) => a(t)));
            AddBehavior(new Behavior<Action<IActor>, IActor>((a, i) => a(i)));
            AddBehavior(new Behavior<Action<IActor,T>, IActor,T>((a, i, t) => a(i,t)));
            AddBehavior(new Behavior<Action>((a) => a())) ;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ActorEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ActorEnumerator<T>(this);
        }

        public void Add(T item)
        {
            this.SendMessage<Action<T>, T>(t => fList.Add(t), item);
        }

        public void Clear()
        {
            this.SendMessage<Action>(() => fList.Clear());
        }

        public bool Contains(T item)
        {
            var future = new Future<bool>();
            this.SendMessage<Action<IActor,T>, IActor,T>((a,t) =>
            {
                a.SendMessage(fList.Contains(t));
            }, future, item);
            return future.Result();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.SendMessage<Action<T[], int>, T[], int>(
                (tab, i) => fList.CopyTo(tab, i),
                array,
                arrayIndex) ;
        }

        public bool Remove(T item)
        {
            var future = new Future<bool>();
            this.SendMessage<Action<IActor, T>, IActor, T>((a, t) =>
            {
                a.SendMessage(fList.Remove(t));
            }, future, item);
            return future.Result();
        }

        private class ActorEnumerator<T> : BaseActor, IEnumerator<T>, IEnumerator, IDisposable
        {
            private EnumerableActor<T> fCollection;

            private enum EnumeratorAction { MoveNext,Reset, Current};

            private int fIndex;

            public ActorEnumerator(EnumerableActor<T> aCollection) : base()
            {
                fCollection = aCollection;
                fIndex = -1;
                Become(new ActionBehavior<IActor>());
            }

            public bool MoveNext()
            {
                var future = new Future<bool>();
                fCollection.SendMessage<Action<IActor>, IActor>((a) =>
                   {
                       fIndex++;
                       a.SendMessage(fIndex < fCollection.fList.Count);
                   }, future) ;
                return future.Result();
            }

            // better than this ?
            public void Reset()
            {
                var future = new Future<bool>();
                this.SendMessage<Action<IActor>, IActor>((a) =>
                {
                    fIndex = -1;
                    a.SendMessage(fIndex);
                }, future);
                future.Result();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposable)
            {
                if (disposable)
                {

                }
            }

            T IEnumerator<T>.Current
            {
                get {
                    var future = new Future<T>();
                    fCollection.SendMessage<Action<IActor>, IActor>((a) =>
                    {
                        a.SendMessage(fCollection.fList[fIndex]);
                    }, future);
                    return future.Result();
                }
            }

            object IEnumerator.Current
            {
                get {
                    var future = new Future<T>();
                    fCollection.SendMessage<Action<IActor>, IActor>((a) =>
                    {
                        a.SendMessage(fCollection.fList[fIndex]);
                    }, future);
                    return future.Result();
                }
            }

        }

    }


}
