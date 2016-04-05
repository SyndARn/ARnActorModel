using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    // state x event = action x state

    public class FsmBehavior<S, E> : Behavior<Tuple<IActor, S, E>>
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

        private bool DoPattern(Tuple<IActor, S, E> aStateEvent)
        {
            return StartState.Equals(aStateEvent.Item2) && (Condition != null && Condition(aStateEvent.Item3));
        }

        private void DoApply(Tuple<IActor, S, E> aStateEvent)
        {
            if (Action != null) Action(aStateEvent.Item3);
            aStateEvent.Item1.SendMessage(EndState);
        }

    }

    public class FsmActor<S, E> : BaseActor
    {
        protected S CurrentState;

        public FsmActor(S StartState, IEnumerable<FsmBehavior<S, E>> someBehaviors) : base()
        {
            CurrentState = StartState;
            Become(new Behavior<S>(ProcessState));
            if (someBehaviors != null)
                foreach (var item in someBehaviors)
                {
                    AddBehavior(item);
                }
        }

        private void ProcessState(S newState)
        {
            CurrentState = newState;
        }
    }

}
