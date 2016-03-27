/*****************************************************************************
		               ARnActor Actor Model Library .Net
     
	 Copyright (C) {2015}  {ARn/SyndARn} 
 
 
     This program is free software; you can redistribute it and/or modify 
     it under the terms of the GNU General Public License as published by 
     the Free Software Foundation; either version 2 of the License, or 
     (at your option) any later version. 
 
 
     This program is distributed in the hope that it will be useful, 
     but WITHOUT ANY WARRANTY; without even the implied warranty of 
     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
     GNU General Public License for more details. 
 
 
     You should have received a copy of the GNU General Public License along 
     with this program; if not, write to the Free Software Foundation, Inc., 
     51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA. 
*****************************************************************************/
using Actor.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Actor.Util
{
    public enum CollectionRequest { Add, Remove, OkAdd, OkRemove } ;

    public class CollectionBehaviors<T> : Behaviors
    {
        internal List<T> List = new List<T>();
        public CollectionBehaviors()
            : base()
        {
            AddBehavior(new AddOrRemoveBehavior<T>());
            AddBehavior(new EnumeratorBehavior<T>());
        }
    }

    public class AddOrRemoveBehavior<T> : Behavior<Tuple<CollectionRequest, T>>
    {

        public AddOrRemoveBehavior()
            : base()
        {
            this.Apply = DoApply;
            this.Pattern = t => { return t is Tuple<CollectionRequest, T>; };
        }

        private void DoApply(Tuple<CollectionRequest, T> Data)
        {
            CollectionBehaviors<T> linkedBehavior = LinkedTo as CollectionBehaviors<T>;
            switch (Data.Item1)
            {
                case CollectionRequest.Add:
                    linkedBehavior.List.Add(Data.Item2);
                    linkedBehavior.LinkedActor.SendMessage(CollectionRequest.OkAdd);
                    break;
                case CollectionRequest.Remove:
                    linkedBehavior.List.Remove(Data.Item2);
                    linkedBehavior.LinkedActor.SendMessage(CollectionRequest.OkRemove);
                    break;
            }
        }
    }

    public enum IteratorMethod { MoveNext, Current, OkCurrent, OkMoveNext } ;

    public class EnumeratorBehavior<T> : Behavior<Tuple<IteratorMethod, int, IActor>>
    {
        public EnumeratorBehavior()
            : base()
        {
            this.Apply = DoApply;
            this.Pattern = t => { return t is Tuple<IteratorMethod, int, IActor>; };
        }

        private void DoApply(Tuple<IteratorMethod, int, IActor> msg)
        {
            CollectionBehaviors<T> linkedBehavior = LinkedTo as CollectionBehaviors<T>;
            switch (msg.Item1)
            {
                case IteratorMethod.MoveNext:
                    {

                        if ((msg.Item2 < linkedBehavior.List.Count) && (msg.Item2 >= 0))
                        {
                            msg.Item3.SendMessage(Tuple.Create(IteratorMethod.OkMoveNext, true));
                        }
                        else
                        {
                            msg.Item3.SendMessage(Tuple.Create(IteratorMethod.OkMoveNext, false));
                        }
                        break;
                    }
                case IteratorMethod.Current:
                    {
                        if ((msg.Item2 >= 0) && (msg.Item2 < linkedBehavior.List.Count))
                            msg.Item3.SendMessage(Tuple.Create(IteratorMethod.OkCurrent, linkedBehavior.List[msg.Item2]));
                        else
                            Debug.WriteLine("Bad current");
                        break;
                    }
                default: throw new ActorException(string.Format(CultureInfo.InvariantCulture,"Bad IteratorMethod call {0}", msg.Item1));
            }
        }
    }

    // (Some prefer this class nested in the collection class.)
    public class CollectionActorEnumerator<T> : ActionActor<T>, IEnumerator<T>, IEnumerator, IDisposable
    {
        private CollectionActor<T> fCollection;

        private int fIndex ;

        public CollectionActorEnumerator(CollectionActor<T> aCollection) : base()
        {
            fCollection = aCollection;
            fIndex = -1;
        }

        public bool MoveNext()
        {
            Interlocked.Increment(ref fIndex);
            var task = Receive(t =>
            {
                var tuple = t as Tuple<IteratorMethod, bool>;
                return tuple != null && tuple.Item1 == IteratorMethod.OkMoveNext; 
            }
                ) ;
            fCollection.SendMessage(Tuple.Create(IteratorMethod.MoveNext, fIndex, (IActor)this));
            
           return (task.Result as Tuple<IteratorMethod, bool>).Item2;
        }

        // better than this ?
        public void Reset() { 
            Interlocked.Exchange(ref fIndex,-1) ; }

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

        public T Current
        {
            get
            {
                var task = Receive(t =>
                {
                    var tuple = t as Tuple<IteratorMethod, T>;
                    return tuple != null &&  tuple.Item1 == IteratorMethod.OkCurrent;
                });
                fCollection.SendMessage(Tuple.Create(IteratorMethod.Current, fIndex, (IActor)this));
                return (task.Result as Tuple<IteratorMethod, T>).Item2;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                var task = Receive(t =>
                {
                    var tu = (Tuple<IteratorMethod, T>)t;
                    return (tu != null) && (tu.Item1 == IteratorMethod.OkCurrent) ;
                });
                fCollection.SendMessage(Tuple.Create(IteratorMethod.Current, fIndex, (IActor)this));
                return (task.Result as Tuple<IteratorMethod, T>).Item2;
                ;
            }
        }

    }

    public class CollectionActor<T> : BaseActor, IEnumerable<T>, IEnumerable
    {
        public CollectionActor()
            : base()
        {
            BecomeMany(new CollectionBehaviors<T>());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CollectionActorEnumerator<T>(this);
        }

        System.Collections.IEnumerator IEnumerable.GetEnumerator()
        {
            return new CollectionActorEnumerator<T>(this);
        }

        public void Add(T aData)
        {
            Receive(t =>
            {
                var val = t is CollectionRequest;
                return val && (CollectionRequest)t == CollectionRequest.OkAdd;
            }) ;
            SendMessage(Tuple.Create(CollectionRequest.Add, aData));
        }

        public void Remove(T aData)
        {
            Receive(t =>
            {
                var val = t is CollectionRequest;
                return val && (CollectionRequest)t == CollectionRequest.OkRemove;
            });
            SendMessage(Tuple.Create(CollectionRequest.Remove, aData));
        }
    }


}

