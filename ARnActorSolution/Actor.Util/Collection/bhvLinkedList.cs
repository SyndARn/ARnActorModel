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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{

    public class actLinkedList<T> : actActor
    {
        public actLinkedList()
            : base()
        {
            BecomeMany(new bhvLinkedList<T>());
        }
    }

    public enum bhvLinkedListOperation { Add, First, Next, Answer } ;

    public class bhvLinkedListAdd<T> : bhvBehavior<Tuple<bhvLinkedListOperation,T>>
    {
        public bhvLinkedListAdd()
            : base()
        {
            Pattern = t => { return bhvLinkedListOperation.Add.Equals(t.Item1) ; };
            Apply = Behavior;
        }

        private void Behavior(Tuple<bhvLinkedListOperation, T> data)
        {
                ((bhvLinkedList<T>)LinkedTo()).fList.AddLast(data.Item2);
        }
    }

    public class bhvLinkedListFirst<T> : bhvBehavior<Tuple<bhvLinkedListOperation, IActor>>
    {
        public bhvLinkedListFirst() : base()
        {
            Pattern = t => { return bhvLinkedListOperation.First.Equals(t.Item1); };
            Apply = Behavior;
        }
        private void Behavior(Tuple<bhvLinkedListOperation, IActor> Sender)
        {
            var first = ((bhvLinkedList<T>)LinkedTo()).fList.First.Value;
            SendMessageTo(Tuple.Create(bhvLinkedListOperation.Answer,first),Sender.Item2);
        }
    }

    public class bhvLinkedListNext<T> : bhvBehavior<Tuple<bhvLinkedListOperation,IActor, T>>
    {
        public bhvLinkedListNext()
            : base()
        {
            Pattern = t => { return bhvLinkedListOperation.Next.Equals(t.Item1); };
            Apply = Behavior;
        }
        private void Behavior(Tuple<bhvLinkedListOperation, IActor, T> data)
        {
            var find = ((bhvLinkedList<T>)LinkedTo()).fList.Find(data.Item3);
            if (find != null)
            {
                var next = find.Next; 
                SendMessageTo(Tuple.Create(bhvLinkedListOperation.Answer,next.Value),data.Item2);
            }
        }
    }

    public class bhvLinkedList<T> : Behaviors
    {
        internal LinkedList<T> fList = new LinkedList<T>();
        public bhvLinkedList()
            : base()
        {
            this.AddBehavior(new bhvLinkedListFirst<T>());
            this.AddBehavior(new bhvLinkedListNext<T>());
            this.AddBehavior(new bhvLinkedListAdd<T>());
        }
    }
}
