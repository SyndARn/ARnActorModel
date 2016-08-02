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

namespace Actor.Util
{

    public class FsmBehavior<TState, TEvent> : Behavior<Tuple<TState, TEvent>>
    {
        public TState StartState { get; private set; }
        public TState EndState { get; private set; }
        public Action<TEvent> Action { get; private set; }
        public Func<TEvent, bool> Condition { get; private set; }

        public FsmBehavior(
            TState origState,
            TState endState,
            Action<TEvent> anAction,
            Func<TEvent, bool> aCondition)
        {
            StartState = origState;
            EndState = endState;
            Action = anAction;
            Condition = aCondition;

            Pattern = DoPattern;
            Apply = DoApply;
        }

        private bool DoPattern(Tuple<TState, TEvent> aStateEvent)
        {
            return StartState.Equals(aStateEvent.Item1) && (Condition != null && Condition(aStateEvent.Item2));
        }

        private void DoApply(Tuple<TState, TEvent> aStateEvent)
        {
            // first change state
            ((FsmActor<TState, TEvent>)LinkedActor).ProcessState(EndState);
            Action?.Invoke(aStateEvent.Item2);
        }
    }

}
