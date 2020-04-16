using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Actor.Base
{
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SimpleActor : IActor
    {
        public ActorTag Tag
        {
            get => _sharedStruct.fTag;
            private set => _sharedStruct.fTag = value;
        } // unique identifier, and host

        private List<IBehavior> _behaviors = new List<IBehavior>(); // our behavior

        private IActorMailBox<object> _mailBox = new ActorMailBox<object>(); // our mailbox
        private SharingStruct _sharedStruct = new SharingStruct();
        public IMessageTracerService MessageTracerService { get; set; }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => ToString();

        public static void CompleteInitialize(SimpleActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor._behaviors = new List<IBehavior>();
            anActor._mailBox = new ActorMailBox<object>();
            if (anActor.Tag != null)
            {
                return;
            }

            anActor.Tag = new ActorTag();
        }

        protected SimpleActor(ActorTag previousTag)
            : base() => Tag = previousTag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask(object msg)
        {
            if (msg != null)
            {
                _mailBox.AddMessage(msg);
            }

            TrySetInTask();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask()
        {
            if (Interlocked.CompareExchange(ref _sharedStruct.fInTask, 1, 0) != 0)
            {
                return;
            }

            ActorTask.AddActor(MessageLoop);
        }

        private void AddMissedMessages()
        {
            // add all missed messages ...
            _mailBox.RefreshFromMissed();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendMessage(object msg)
        {
            TrySetInTask(msg);
            MessageTracerService?.TraceMessage(msg);
            GlobalContext.MessageTracerService?.TraceMessage(msg);
        }

        public static SimpleActor Add(SimpleActor anActor, object aMessage)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(aMessage);
            return anActor;
        }

        public static SimpleActor operator +(SimpleActor anActor, object aMessage) => Add(anActor, aMessage);

        public SimpleActor(IBehaviors someBehaviors)
        {
            CheckArg.Behaviors(someBehaviors);
            Tag = new ActorTag();
            someBehaviors.LinkToActor(this);
            _behaviors.AddRange(someBehaviors.AllBehaviors());
        }

        public SimpleActor(IBehavior aBehavior)
        {
            Tag = new ActorTag();
            Behaviors someBehaviors = new Behaviors();
            someBehaviors.LinkToActor(this);
            _behaviors.Add(aBehavior);
        }

        public SimpleActor(IBehavior[] manyBehaviors)
        {
            CheckArg.BehaviorParam(manyBehaviors);
            Tag = new ActorTag();
            Behaviors someBehaviors = new Behaviors();
            foreach (IBehavior item in manyBehaviors)
            {
                someBehaviors.AddBehavior(item);
                _behaviors.Add(item);
            }

            someBehaviors.LinkToActor(this);
        }

        public SimpleActor() => Tag = new ActorTag();

        private bool ReceiveMessage(out object message)
        {
            message = _mailBox.GetMessage();
            return message != null;
        }

        protected void Become(IBehaviors someBehaviors)
        {
            CheckArg.Behaviors(someBehaviors);
            _behaviors.Clear();
            someBehaviors.LinkToActor(this);
            _behaviors.AddRange(someBehaviors.AllBehaviors());
            AddMissedMessages();
            TrySetInTask();
        }

        protected void Become(params IBehavior[] manyBehaviors)
        {
            CheckArg.BehaviorParam(manyBehaviors);

            _behaviors.Clear();

            Behaviors someBehaviors = new Behaviors(manyBehaviors);
            _behaviors.AddRange(manyBehaviors);
            someBehaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void AddBehavior(IBehaviors someBehaviors)
        {
            CheckArg.Behaviors(someBehaviors);
            someBehaviors.LinkToActor(this);
            _behaviors.AddRange(someBehaviors.AllBehaviors());
            AddMissedMessages();
            TrySetInTask();
        }

        protected void AddBehavior(params IBehavior[] someBehaviors)
        {
            CheckArg.BehaviorParam(someBehaviors);
            Behaviors behaviors = new Behaviors();

            foreach (IBehavior item in someBehaviors)
            {
                behaviors.AddBehavior(item);
                _behaviors.Add(item);
            }

            behaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void RemoveBehavior(IBehavior aBehavior)
        {
            CheckArg.Behavior(aBehavior);
            AddMissedMessages();
            _behaviors.Remove(aBehavior);
        }

        private void MessageLoop()
        {

            while (ReceiveMessage(out object msg))
            {
                IBehavior behavior = MatchByPattern(msg);

                if (behavior == null)
                {
                    _mailBox.AddMiss(msg);
                }
                else
                {
                    behavior.StandardApply(msg);
                }
            }

            Interlocked.Exchange(ref _sharedStruct.fInTask, 0);

            if (_mailBox.IsEmpty)
            {
                return;
            }

            TrySetInTask();
        }

        private IBehavior MatchByPattern(object msg)
        {
            foreach (IBehavior fBehavior in _behaviors)
            {
                if (fBehavior?.StandardPattern(msg) == true)
                {
                    return fBehavior;
                }
            }

            return null;
        }
    }
}
