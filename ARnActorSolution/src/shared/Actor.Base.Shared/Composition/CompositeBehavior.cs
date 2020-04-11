using System.Collections.Generic;

namespace Actor.Base
{
    public abstract class CompositeBehavior<T> : Behaviors
    {
        protected HashSet<IActor> inputs = new HashSet<IActor>();
        protected HashSet<IActor> outputs = new HashSet<IActor>();

        public void RegisterInput(IActor actor) => LinkedActor.SendMessage(actor, true, true);

        public void UnregisterInput(IActor actor) => LinkedActor.SendMessage(actor, true, false);

        public void RegisterOutput(IActor actor) => LinkedActor.SendMessage(actor, false, true);

        public void UnregisterOutput(IActor actor) => LinkedActor.SendMessage(actor, false, false);

        protected CompositeBehavior()
        {
            BecomeBehavior(new Behavior<IActor, T>(
                (i,t) => (i is IActor) && (t is T) && (inputs.Contains(i)),
                DoCompositeBehavior));
            AddBehavior(new Behavior<IActor, bool, bool>(DoRegisterActor));
        }

        private void DoRegisterActor(IActor actor, bool input, bool register)
        {
            if (input)
            {
                if (register)
                {
                    inputs.Add(actor);
                }
                else
                {
                    inputs.Remove(actor);
                }
            }
            else
            {
                if (register)
                {
                    outputs.Add(actor);
                }
                else
                {
                    outputs.Remove(actor);
                }
            }
        }

        protected abstract void DoCompositeBehavior(IActor actor, T t);
    }
}
