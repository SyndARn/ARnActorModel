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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{
    public enum CollectionRequest { Add, Remove, OkAdd, OkRemove } ;

    public class bhvCollection<T> : Behaviors
    {
        internal List<T> List = new List<T>();
        public bhvCollection()
            : base()
        {
            AddBehavior(new bhvAddOrRemoveBehavior<T>());
            AddBehavior(new bhvEnumeratorBehavior<T>());
        }
    }

    public class bhvAddOrRemoveBehavior<T> : bhvBehavior<Tuple<CollectionRequest, T>>
    {

        public bhvAddOrRemoveBehavior()
            : base()
        {
            this.Apply = DoApply;
            this.Pattern = t => { return t is Tuple<CollectionRequest, T>; };
        }

        private void DoApply(Tuple<CollectionRequest, T> Data)
        {
            bhvCollection<T> linkedBehavior = LinkedTo() as bhvCollection<T>;
            switch (Data.Item1)
            {
                case CollectionRequest.Add:
                    linkedBehavior.List.Add(Data.Item2);
                    SendMessageTo(CollectionRequest.OkAdd, linkedBehavior.LinkedActor);
                    break;
                case CollectionRequest.Remove:
                    linkedBehavior.List.Remove(Data.Item2);
                    SendMessageTo(CollectionRequest.OkRemove, linkedBehavior.LinkedActor);
                    break;
            }
        }
    }

    public enum IteratorMethod { MoveNext, Current, OkCurrent, OkMoveNext } ;

    public class bhvEnumeratorBehavior<T> : bhvBehavior<Tuple<IteratorMethod, int, IActor>>
    {
        public bhvEnumeratorBehavior()
            : base()
        {
            this.Apply = DoApply;
            this.Pattern = t => { return t is Tuple<IteratorMethod, int, IActor>; };
        }

        private void DoApply(Tuple<IteratorMethod, int, IActor> msg)
        {
            bhvCollection<T> linkedBehavior = LinkedTo() as bhvCollection<T>;
            switch (msg.Item1)
            {
                case IteratorMethod.MoveNext:
                    {

                        if ((msg.Item2 < linkedBehavior.List.Count) && (msg.Item2 >= 0))
                        {
                            SendMessageTo(Tuple.Create(IteratorMethod.OkMoveNext, true), msg.Item3);
                        }
                        else
                        {
                            SendMessageTo(Tuple.Create(IteratorMethod.OkMoveNext, false), msg.Item3);
                        }
                        break;
                    }
                case IteratorMethod.Current:
                    {
                        if ((msg.Item2 >= 0) && (msg.Item2 < linkedBehavior.List.Count))
                            SendMessageTo(Tuple.Create(IteratorMethod.OkCurrent, linkedBehavior.List[msg.Item2]), msg.Item3);
                        else
                            Debug.WriteLine("Bad current");
                        break;
                    }
                default: throw new Exception(string.Format("Bad IteratorMethod call {0}", msg.Item1));
            }
        }
    }

    // (Some prefer this class nested in the collection class.)
    public class actCollectionEnumerator<T> : actAction<T>, IEnumerator<T>, IEnumerator
    {
        private actCollection<T> fCollection;

        private int fIndex = -1;

        public actCollectionEnumerator(actCollection<T> aCollection)
        {
            fCollection = aCollection;
        }

        public bool MoveNext()
        {
            fIndex++;
            SendMessageTo(Tuple.Create(IteratorMethod.MoveNext, fIndex, (IActor)this), fCollection);
            Tuple<IteratorMethod, bool> retval = Receive(t =>
            { return (t is Tuple<IteratorMethod, bool>) && ((Tuple<IteratorMethod, bool>)t).Item1 == IteratorMethod.OkMoveNext; }
                ).Result as Tuple<IteratorMethod, bool>;
            return retval.Item2;
        }

        // better than this ?
        public void Reset() { fIndex = -1; }

        void IDisposable.Dispose()
        {
        }

        public T Current
        {
            get
            {
                SendMessageTo(Tuple.Create(IteratorMethod.Current, fIndex, (IActor)this), fCollection);
                Tuple<IteratorMethod, T> retval = Receive(t =>
                {
                    return (t is Tuple<IteratorMethod, T>) && ((Tuple<IteratorMethod, T>)t).Item1 == IteratorMethod.OkCurrent;
                }
                    ).Result as Tuple<IteratorMethod, T>;
                return retval.Item2;
            }
        }


        object IEnumerator.Current
        {
            get
            {
                SendMessageTo(Tuple.Create(IteratorMethod.Current, fIndex, (IActor)this), fCollection);
                Tuple<IteratorMethod, T> retval = Receive(t =>
                {
                    return (t is Tuple<IteratorMethod, T>) && ((Tuple<IteratorMethod, T>)t).Item1 == IteratorMethod.OkCurrent;
                }
                ).Result as Tuple<IteratorMethod, T>;
                return retval.Item2;
                ;
            }
        }


    }


    public class actCollection<T> : actActor, IEnumerable<T>
    {
        private static int fCount = 0;
        public actCollection()
            : base()
        {
            BecomeMany(new bhvCollection<T>());
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new actCollectionEnumerator<T>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new actCollectionEnumerator<T>(this);
        }

        public void Add(T aData)
        {
            fCount++;
            SendMessageTo(Tuple.Create(CollectionRequest.Add, aData));
            var retval = Receive(t =>
            {
                var val = t is CollectionRequest;
                return val && (CollectionRequest)t == CollectionRequest.OkAdd;
            }).Result;
        }

        public void Remove(T aData)
        {
            SendMessageTo(Tuple.Create(CollectionRequest.Remove, aData));
            var retval = Receive(t =>
            {
                var val = t is CollectionRequest;
                return val && (CollectionRequest)t == CollectionRequest.OkRemove;
            }).Result;
        }
    }


}

