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
    public class FsmBehaviors<TState, TEvent> : Behaviors
    {
        private TState _current { get; set; }

        private bool fBehaviorSet;

        public FsmBehaviors() : base()
        {
        }

        internal TState GetCurrentState()
        {
            return _current;
        }

        internal void ChangeState(TState aState)
        {
            _current = aState;
        }

        private void GetCurrentState(IFuture<TState> future)
        {
            future.SendMessage(_current);
        }

        public FsmBehaviors<TState, TEvent> AddRule(TState startState, Func<TEvent, bool> aCondition, Action<TEvent> anAction, TState reachedState)
        {
            return AddRule(startState, aCondition, anAction, reachedState, null);
        }

        public FsmBehaviors<TState, TEvent> AddRule(TState startState, Func<TEvent, bool> aCondition, Action<TEvent> anAction, TState reachedState, IActor traceActor)
        {
            if (!fBehaviorSet)
            {
                _current = startState;
                BecomeBehavior(new Behavior<IFuture<TState>>(GetCurrentState)) ;
                fBehaviorSet = true;
            }
            AddBehavior(new FsmBehavior<TState, TEvent>
                (
                    startState,
                    reachedState,
                    anAction,
                    aCondition,
                    traceActor
                ));
            return this;
        }
    }

    public class FsmBehavior<TState, TEvent> : Behavior<TEvent>
    {
        public TState StartState { get; private set; }
        public TState EndState { get; private set; }
        public Action<TEvent> Action { get; private set; }
        public Func<TEvent, bool> Condition { get; private set; }

        public IActor TraceActor { get; private set; }

        public FsmBehavior(
            TState origState,
            TState endState,
            Action<TEvent> anAction,
            Func<TEvent, bool> aCondition,
            IActor traceActor)
        {
            StartState = origState;
            EndState = endState;
            Action = anAction;
            Condition = aCondition;
            TraceActor = traceActor;

            Pattern = DoPattern;
            Apply = DoApply;
        }

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
            TraceActor = null;

            Pattern = DoPattern;
            Apply = DoApply;
        }

        private bool DoPattern(TEvent anEvent)
        {
            var parent = LinkedTo as FsmBehaviors<TState, TEvent>;
            var result = parent == null ?
                false :
                parent.GetCurrentState().Equals(StartState) && (Condition == null || Condition(anEvent));
            TraceActor?.SendMessage(StartState, EndState, anEvent.ToString());
            return result;
        }

        private void DoApply(TEvent anEvent)
        {
            if (LinkedTo is FsmBehaviors<TState, TEvent> parent)
            {
                // first change state in parent behaviors
                parent.ChangeState(EndState);
                Action?.Invoke(anEvent);
            }
        }
    }
}
