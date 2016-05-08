using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    // state x event = action x state

    public class FsmBehavior<S, E> : Behavior<Tuple<S, E>>
    {
        public S StartState { get; private set; }
        public S EndState { get; private set; }
        public Action<E> Action { get; private set; }
        public Func<E,bool> Condition { get; private set; }

        public FsmBehavior(
            S origState,
            S endState,
            Action<E> anAction,
            Func<E,bool> aCondition)
        {
            StartState = origState;
            EndState = endState;
            Action = anAction;
            Condition = aCondition;

            Pattern = DoPattern;
            Apply = DoApply;
        }

        private bool DoPattern(Tuple<S, E> aStateEvent)
        {
            return StartState.Equals(aStateEvent.Item1) && (Condition != null && Condition(aStateEvent.Item2));
        }

        private void DoApply(Tuple<S, E> aStateEvent)
        {
            // first change state
            ((FsmActor<S, E>)LinkedActor).ProcessState(EndState);
            if (Action != null) Action(aStateEvent.Item2);
            // this.LinkedActor.SendMessage(EndState);
        }

    }

    public class FsmActor<S, E> : BaseActor
    {
        protected S CurrentState { get; set; }

        public FsmActor(S StartState, IEnumerable<FsmBehavior<S, E>> someBehaviors) : base()
        {
            CurrentState = StartState;
            Become(new Behavior<S>(ProcessState));
            AddBehavior(new Behavior<Tuple<IActor, S>>(GetState));
            if (someBehaviors != null)
                foreach (var item in someBehaviors)
                {
                    AddBehavior(item);
                }
        }

        private void GetState(Tuple<IActor, S> sender)
        {
            sender.Item1.SendMessage(new Tuple<IActor, S>(this, CurrentState));
        }

        internal void ProcessState(S newState)
        {
            CurrentState = newState;
        }

        public Future<Tuple<IActor,S>> GetCurrentState()
        {
            var future = new Future<Tuple<IActor,S>>();
            SendMessage(new Tuple<IActor, S>(future,CurrentState));
            return future;
        }
    }

}
