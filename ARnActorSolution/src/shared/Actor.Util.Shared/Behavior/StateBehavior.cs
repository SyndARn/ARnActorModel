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

    public class StateFullActor<T> : BaseActor, IStateFullActor<T>
    {
        public StateFullActor() : base()
        {
            Become(new StateBehaviors<T>());
        }




        public void SetState(T aT)
        {
            this.SendMessage(StateAction.Set, aT);
        }

        public IFuture<T> GetState()
        {
            IFuture<T> future = new Future<T>();
            this.SendMessage(StateAction.Get, future);
            return future;
        }

        public async Task<T> GetStateAsync()
        {
            this.SendMessage(StateAction.Get);
            return (T) await Receive(t => { return t is T; }) ;
        }

        public async Task<T> GetStateAsync(int timeOutMS)
        {
            this.SendMessage(StateAction.Get);
            return (T)await Receive(t => { return t is T; }, timeOutMS);
        }
    }

    public enum StateAction { Set, Get } ;

    public class StateBehaviors<T> : Behaviors
    {
        internal T Value { get; set; }
        public StateBehaviors() : base()
        {
            AddBehavior(new SetStateBehavior<T>());
            AddBehavior(new GetStateBehavior<T>());
            AddBehavior(new GetStateBehaviorFuture<T>());
        }
    }

    public class SetStateBehavior<T> : Behavior<StateAction, T>
    {
        public SetStateBehavior()
            : base()
        {
            Pattern = (s, t) => s == StateAction.Set;
            Apply = (s, t) => SetValue(t);
        }

        private void SetValue(T msg)
        {
            ((StateBehaviors<T>)LinkedTo).Value = msg;
        }

    }

    public class GetStateBehaviorFuture<T> : Behavior<StateAction, IFuture<T>>
    {
        public GetStateBehaviorFuture()
            : base()
        {
            Pattern = (s,f) => s == StateAction.Get;
            Apply = (s,f) => GetValue(f);
        }

        private void GetValue(IFuture<T> future)
        {
            future.SendMessage(((StateBehaviors<T>)LinkedTo).Value);
        }
    }

    public class GetStateBehavior<T> : Behavior<StateAction>
    {
        public GetStateBehavior()
            : base()
        {
            Pattern = s => s == StateAction.Get ;
            Apply = s => GetValue();
        }

        private void GetValue()
        {
            LinkedActor.SendMessage(((StateBehaviors<T>)LinkedTo).Value);
        }
    }

}
