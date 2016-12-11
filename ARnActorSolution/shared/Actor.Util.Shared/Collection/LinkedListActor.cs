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
using System.Collections.Generic;
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

    public class LinkedListAddbehavior<T> : Behavior<LinkedListOperation, T>
    {
        public LinkedListAddbehavior()
            : base()
        {
            Pattern = (l,t) => { return LinkedListOperation.Add.Equals(l) ; };
            Apply = Behavior;
        }

        private void Behavior(LinkedListOperation operation, T data)
        {
                ((LinkedListBehaviors<T>)LinkedTo).fList.AddLast(data);
        }
    }

    public class BehaviorLinkedListFirst<T> : Behavior<LinkedListOperation, IActor>
    {
        public BehaviorLinkedListFirst() : base()
        {
            Pattern = (l,t) => { return LinkedListOperation.First.Equals(l); };
            Apply = Behavior;
        }
        private void Behavior(LinkedListOperation operation, IActor Sender)
        {
            var first = ((LinkedListBehaviors<T>)LinkedTo).fList.First.Value;
            Sender.SendMessage(LinkedListOperation.Answer, first);
        }
    }

    public class BehaviorLinkedListNext<T> : Behavior<LinkedListOperation, IActor, T>
    {
        public BehaviorLinkedListNext()
            : base()
        {
            Pattern = (l,i,t) => { return LinkedListOperation.Next.Equals(l); };
            Apply = Behavior;
        }
        private void Behavior(LinkedListOperation operation, IActor actor, T data)
        {
            var find = ((LinkedListBehaviors<T>)LinkedTo).fList.Find(data);
            if (find != null)
            {
                var next = find.Next;
                actor.SendMessage(LinkedListOperation.Answer, next.Value);
            }
        }
    }

    public class LinkedListBehaviors<T> : Behaviors
    {
        internal LinkedList<T> fList = new LinkedList<T>();
        public LinkedListBehaviors()
            : base()
        {
            this.AddBehavior(new BehaviorLinkedListFirst<T>());
            this.AddBehavior(new BehaviorLinkedListNext<T>());
            this.AddBehavior(new LinkedListAddbehavior<T>());
        }
    }
}
