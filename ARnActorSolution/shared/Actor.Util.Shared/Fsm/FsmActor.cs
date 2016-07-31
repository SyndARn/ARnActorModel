/*****************************************************************************
		               ARnActor Actor Model Library C# .Net
     
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
using System.Collections.Generic;

namespace Actor.Util
{

    public class FsmActor<TState, TEvent> : BaseActor
    {
        protected TState InternalCurrentState { get; set; }

        public FsmActor(TState startState, IEnumerable<FsmBehavior<TState, TEvent>> someBehaviors) : base()
        {
            InternalCurrentState = startState;
            Become(new Behavior<TState>(ProcessState));
            AddBehavior(new Behavior<Tuple<IActor, TState>>(GetState));
            if (someBehaviors != null)
                foreach (var item in someBehaviors)
                {
                    AddBehavior(item);
                }
        }

        private void GetState(Tuple<IActor, TState> sender)
        {
            sender.Item1.SendMessage(InternalCurrentState);
        }

        // TODO check this
        internal void ProcessState(TState newState)
        {
            InternalCurrentState = newState;
        }

        public Future<TState> GetCurrentState()
        {
            var future = new Future<TState>();
            SendMessage(new Tuple<IActor, TState>(future,InternalCurrentState));
            return future;
        }
    }

}
