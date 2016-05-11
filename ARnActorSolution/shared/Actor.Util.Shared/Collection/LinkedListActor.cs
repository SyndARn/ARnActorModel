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

    public class LinkedListActor<T> : BaseActor
    {
        public LinkedListActor()
            : base()
        {
            Become(new LinkedListBehaviors<T>());
        }
    }

    public enum LinkedListOperation { Add, First, Next, Answer } ;

    public class LinkedListAddbehavior<T> : Behavior<Tuple<LinkedListOperation, T>>
    {
        public LinkedListAddbehavior()
            : base()
        {
            Pattern = t => { return LinkedListOperation.Add.Equals(t.Item1) ; };
            Apply = Behavior;
        }

        private void Behavior(Tuple<LinkedListOperation, T> data)
        {
                ((LinkedListBehaviors<T>)LinkedTo).fList.AddLast(data.Item2);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvLinkedListFirst<T> : Behavior<Tuple<LinkedListOperation, IActor>>
    {
        public bhvLinkedListFirst() : base()
        {
            Pattern = t => { return LinkedListOperation.First.Equals(t.Item1); };
            Apply = Behavior;
        }
        private void Behavior(Tuple<LinkedListOperation, IActor> Sender)
        {
            var first = ((LinkedListBehaviors<T>)LinkedTo).fList.First.Value;
            Sender.Item2.SendMessage(Tuple.Create(LinkedListOperation.Answer, first));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvLinkedListNext<T> : Behavior<Tuple<LinkedListOperation, IActor, T>>
    {
        public bhvLinkedListNext()
            : base()
        {
            Pattern = t => { return LinkedListOperation.Next.Equals(t.Item1); };
            Apply = Behavior;
        }
        private void Behavior(Tuple<LinkedListOperation, IActor, T> data)
        {
            var find = ((LinkedListBehaviors<T>)LinkedTo).fList.Find(data.Item3);
            if (find != null)
            {
                var next = find.Next;
                data.Item2.SendMessage(Tuple.Create(LinkedListOperation.Answer, next.Value));
            }
        }
    }

    public class LinkedListBehaviors<T> : Behaviors
    {
        internal LinkedList<T> fList = new LinkedList<T>();
        public LinkedListBehaviors()
            : base()
        {
            this.AddBehavior(new bhvLinkedListFirst<T>());
            this.AddBehavior(new bhvLinkedListNext<T>());
            this.AddBehavior(new LinkedListAddbehavior<T>());
        }
    }
}
