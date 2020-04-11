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
    public enum CollectionRequest { Add, Remove, OkAdd, OkRemove };

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

    public class AddOrRemoveBehavior<T> : Behavior<CollectionRequest, T>
    {
        public AddOrRemoveBehavior()
            : base()
        {
            this.Pattern = DefaultPattern();
            this.Apply = DoApply;
        }

        private void DoApply(CollectionRequest request, T Data)
        {
            CollectionBehaviors<T> linkedBehavior = LinkedTo as CollectionBehaviors<T>;
            switch (request)
            {
                case CollectionRequest.Add:
                    linkedBehavior.List.Add(Data);
                    linkedBehavior.LinkedActor.SendMessage(CollectionRequest.OkAdd);
                    break;
                case CollectionRequest.Remove:
                    linkedBehavior.List.Remove(Data);
                    linkedBehavior.LinkedActor.SendMessage(CollectionRequest.OkRemove);
                    break;
            }
        }
    }

    public enum IteratorMethod { MoveNext, Current, OkCurrent, OkMoveNext };

    public class EnumeratorBehavior<T> : Behavior<IteratorMethod, int, IActor>
    {
        public EnumeratorBehavior()
            : base()
        {
            this.Pattern = DefaultPattern();
            this.Apply = DoApply;
        }

        private void DoApply(IteratorMethod method, int i, IActor actor)
        {
            CollectionBehaviors<T> linkedBehavior = LinkedTo as CollectionBehaviors<T>;
            switch (method)
            {
                case IteratorMethod.MoveNext:
                    {
                        if ((i < linkedBehavior.List.Count) && (i >= 0))
                        {
                            actor.SendMessage(IteratorMethod.OkMoveNext, true);
                        }
                        else
                        {
                            actor.SendMessage(IteratorMethod.OkMoveNext, false);
                        }
                        break;
                    }
                case IteratorMethod.Current:
                    {
                        if ((i >= 0) && (i < linkedBehavior.List.Count))
                            actor.SendMessage(IteratorMethod.OkCurrent, linkedBehavior.List[i]);
                        else
                            Debug.WriteLine("Bad current");
                        break;
                    }
                default: throw new ActorException(string.Format(CultureInfo.InvariantCulture, "Bad IteratorMethod call {0}", method));
            }
        }
    }

    public class CollectionActorEnumerator<T> : ActionActor<T>, IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly CollectionActor<T> fCollection;

        private int fIndex;

        public CollectionActorEnumerator(CollectionActor<T> aCollection) : base()
        {
            fCollection = aCollection;
            fIndex = -1;
        }

        public bool MoveNext()
        {
            Interlocked.Increment(ref fIndex);
            var task = Receive(t => t is IMessageParam<IteratorMethod, bool> messageParam && messageParam.Item1 == IteratorMethod.OkMoveNext
                );
            fCollection.SendMessage(IteratorMethod.MoveNext, fIndex, this);

            return (task.Result as IMessageParam<IteratorMethod, bool>).Item2;
        }

        // better than this ?
        public void Reset()
        {
            Interlocked.Exchange(ref fIndex, -1);
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

        public T Current
        {
            get
            {
                var task = Receive(t =>
                {
                    var messageParam = t as IMessageParam<IteratorMethod, T>;
                    return messageParam?.Item1 == IteratorMethod.OkCurrent;
                });
                fCollection.SendMessage(IteratorMethod.Current, fIndex, this);
                return (task.Result as IMessageParam<IteratorMethod, T>).Item2;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                var task = Receive(t =>
                {
                    var tu = (IMessageParam<IteratorMethod, T>)t;
                    return tu?.Item1 == IteratorMethod.OkCurrent;
                });
                fCollection.SendMessage(IteratorMethod.Current, fIndex, (IActor)this);
                return (task.Result as IMessageParam<IteratorMethod, T>).Item2;
            }
        }
    }

    public class CollectionActor<T> : BaseActor, IEnumerable<T>, IEnumerable
    {
        public CollectionActor()
            : base()
        {
            Become(new CollectionBehaviors<T>());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CollectionActorEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CollectionActorEnumerator<T>(this);
        }

        public async void Add(T aData)
        {
            this.SendMessage(CollectionRequest.Add, aData);
            await Receive(t =>
            {
                var val = t is CollectionRequest;
                return val && (CollectionRequest)t == CollectionRequest.OkAdd;
            }).ConfigureAwait(false);
        }

        public async void Remove(T aData)
        {
            this.SendMessage(CollectionRequest.Remove, aData);
            await Receive(t =>
            {
                var val = t is CollectionRequest;
                return val && (CollectionRequest)t == CollectionRequest.OkRemove;
            }).ConfigureAwait(false);
        }
    }
}

