using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Actor.Base
{

    // composing actor ...
    /// <summary>
    /// actor have a default target
    /// when use in composition
    /// this default target could be use to make relay
    /// message should transport this default target to enable the real sender
    /// message should handle sender,target,relaysender,relaytarget
    /// with relaysender and relaytarget, you could have a composition
    /// </summary>

    // hold the pattern to be used in receive mode
    // protected by fReceiveMode
    internal class PatternFuture
    {
        public Func<Object, bool> Pattern;
        public TaskCompletionSource<Object> TaskCompletion;
        public Object Message;
        public Action<Object> Behavior;
    }

    public class actActor : IActor//, IDisposable
    {
        public actTag Tag { get; private set; } // unique identifier, and host
        private Behaviors fBehaviors; // our behavior
        private ActorMailBox fMailBox = new ActorMailBox(); // our mailbox
        private Queue<PatternFuture> fTCSQueue; // only allocate if needed in receive mode, protected by fTaskReserved
        private int fInTask = 0; // 0 out of task, 1 in task
        private int fTaskReserved = 0; // queue pattern lock
        private int fReceiveMode = 0; // receive mode
        private int fRunning = 0; // one active loop
        private IActor fRedirector = null;
        private int fPatternChange = 0;
        private int messCount; // this should always be queue + postpone total

        public bool IsRemote()
        {
            return Tag.IsRemote;
        }

        public static void CompleteInitialize(actActor anActor)
        {
            anActor.fMailBox = new ActorMailBox();
            if (anActor.Tag == null)
            {
                anActor.Tag = new actTag();
            }
        }

        protected actActor(actTag previousTag)
            : base()
        {
            Tag = previousTag;
        }

        public void RedirectTo(IActor anActor)
        {
            IActor aRedirector = new actRedirector(anActor);
            // this could be threaded ...
            Interlocked.Exchange(ref fRedirector, aRedirector);
        }

        private void AddMessage(Object aMessage)
        {
            fMailBox.AddMessage(aMessage);
            IncMess();
        }

        private void TrySetInTask(Object msg)
        {

            if (msg != null)
            {
                AddMessage(msg);
            }
            if (Interlocked.CompareExchange(ref fInTask, 1, 0) == 0)
            {
                ActorTask.AddActor(this);
            }
            else
            {
                if (Interlocked.CompareExchange(ref fReceiveMode, fReceiveMode, fReceiveMode) > 0)
                {
                    if (msg != null)
                    {
                        ActorTask.AddActor(this);
                    }
                }
            }
        }

        private void AddMissedMessages()
        {
            // add all missed messages ...
            Interlocked.Add(ref messCount, fMailBox.RefreshFromMissed());
        }

        private static void DoSendMessageTo(Object msg, IActor aTargetActor)
        {
            ((actActor)aTargetActor).TrySetInTask(msg);
        }

        protected internal void SendMessageTo(Object msg, IActor aTargetActor = null)
        {
            if (fRedirector != null)
            {
                DoSendMessageTo(new RedirectMessage(msg, aTargetActor == null ? this : aTargetActor), fRedirector);
            }
            else
            {
                DoSendMessageTo(msg, aTargetActor == null ? this : aTargetActor);
            }
        }

        public actActor(Behaviors someBehaviors)
        {
            Tag = new actTag();
            BecomeMany(someBehaviors);
        }

        public actActor(IBehavior aBehavior)
        {
            Tag = new actTag();
            Become(aBehavior);
        }

        public actActor()
        {
            Tag = new actTag();
        }

        protected async Task<Object> Receive(Func<Object, bool> aPattern)
        {
            Interlocked.Increment(ref fReceiveMode);
            object ret = null;
            while (ret == null)
            {
                SpinWait sw = new SpinWait();
                while (Interlocked.CompareExchange(ref fTaskReserved, 1, 0) != 0)
                {
                    sw.SpinOnce();
                }
                var lTCS = new PatternFuture();
                lTCS.Pattern = aPattern;
                lTCS.TaskCompletion = new TaskCompletionSource<Object>();
                if (fTCSQueue == null)
                {
                    fTCSQueue = new Queue<PatternFuture>();
                }
                fTCSQueue.Enqueue(lTCS);
                Interlocked.Increment(ref fPatternChange);
                Interlocked.Exchange(ref fTaskReserved, 0);
                Interlocked.Exchange(ref fRunning, 0);
                TrySetInTask(null);
                ret = await lTCS.TaskCompletion.Task;
                Interlocked.Exchange(ref fRunning, 1);
            }
            Interlocked.Decrement(ref fReceiveMode);
            return ret;
        }


        internal void Run()
        {
            while 
            // if
                (Interlocked.CompareExchange(ref fRunning, 1, 0) == 0)
            {
                PatternFuture tcs = null;
                if (Interlocked.Exchange(ref fPatternChange, 0) > 0)
                {
                    AddMissedMessages();
                }
                while ((tcs == null) && (Interlocked.CompareExchange(ref messCount, 0, 0) != 0))
                {
                    // message
                    Object msg = ReceiveMessage();
                    if (msg == null)
                        Debug.WriteLine("null message received");
                    else
                    {

                        #region runpart
                        if (tcs == null)
                        {
                            if (fBehaviors != null)
                            {
                                foreach (IBehavior Behavior in fBehaviors.GetBehaviors())
                                {
                                    if (Behavior != null)
                                    {
                                        if (Behavior.StandardPattern(msg))
                                        {
                                            tcs = new PatternFuture();
                                            tcs.Message = msg;
                                            tcs.Behavior = Behavior.StandardApply;
                                            break; // at least one pattern apply
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region receivepart
                        if (tcs == null)
                        {
                            if (Interlocked.CompareExchange(ref fReceiveMode, 0, 0) != 0)
                            {
                                SpinWait spin = new SpinWait();
                                while (Interlocked.CompareExchange(ref fTaskReserved, 1, 0) != 0)
                                {
                                    spin.SpinOnce();
                                }
                                Queue<PatternFuture> list = new Queue<PatternFuture>();
                                while ((tcs == null) && (fTCSQueue != null) && (fTCSQueue.Count > 0))
                                {
                                    tcs = fTCSQueue.Dequeue();
                                    if (tcs.Pattern(msg))
                                    {
                                        tcs.Message = msg;
                                        AddMissedMessages();
                                    }
                                    else
                                    {
                                        list.Enqueue(tcs);
                                        tcs = null;
                                    }
                                }
                                while (list.Count > 0)
                                {
                                    fTCSQueue.Enqueue(list.Dequeue());
                                }
                                Interlocked.Exchange(ref fTaskReserved, 0);
                            }
                        }
                        #endregion


                        // no pattern apply, this message is missed
                        if (tcs == null)
                        {
                            fMailBox.AddMiss(msg);
                        }
                    }
                }

                if (tcs != null)
                {
                    if (tcs.TaskCompletion == null)
                    {
                        tcs.Behavior(tcs.Message);
                    }
                    else
                    {
                        AddMissedMessages();
                        tcs.TaskCompletion.SetResult(tcs.Message);
                    }
                    if (Interlocked.Exchange(ref fPatternChange, 0) > 0)
                    {
                        AddMissedMessages();
                    }
                }

                Interlocked.Exchange(ref fRunning, 0);
                if (Interlocked.CompareExchange(ref messCount, 0, 0) == 0)
                {
                    break;
                }
            }

            Interlocked.Exchange(ref fInTask, 0);

            // there may be some message that arrives before the set out task but after the while condition
            if (Interlocked.CompareExchange(ref messCount, 0, 0) != 0)
            {
                TrySetInTask(null);
            }

        }

        private void IncMess()
        {
            Interlocked.Increment(ref messCount);
        }

        private void DecMess()
        {
            Interlocked.Decrement(ref messCount);
        }

        private Object ReceiveMessage()
        {
            Object msg = fMailBox.GetMessage();
            if (msg != null)
            {
                DecMess();
            }
            else
            {
                Debug.WriteLine("null message");
            }
            return msg;
        }

        protected void BecomeMany(Behaviors someBehavior)
        {
            fBehaviors = someBehavior;
            fBehaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask(null);
        }

        protected void Become(IBehavior aBehavior)
        {
            fBehaviors = new Behaviors();
            fBehaviors.AddBehavior(aBehavior);
            fBehaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask(null);
        }

        protected void AddBehavior(IBehavior aBehavior)
        {
            fBehaviors.AddBehavior(aBehavior);
            AddMissedMessages();
            TrySetInTask(null);
        }

        protected void RemoveBehavior(IBehavior aBehavior)
        {
            AddMissedMessages();
            fBehaviors.RemoveBehavior(aBehavior);
        }

    }

}
