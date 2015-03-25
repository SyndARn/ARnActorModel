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
        public TaskCompletionSource<Object> Message;
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
        private IActor fRedirector = null;
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
        }

        private void AddRunMissedMessages()
        {
            // add all missed messages ...
            Interlocked.Add(ref messCount, fMailBox.RefreshFromRunMissed());
        }

        private void AddReceiveMissedMessages()
        {
            // add all missed messages ...
            Interlocked.Add(ref messCount, fMailBox.RefreshFromReceiveMissed());
        }

        private bool TryReceive()
        {
            Object ret = null;
            SpinWait sw = new SpinWait();
            while (Interlocked.CompareExchange(ref fTaskReserved, 1, 0) != 0)
            {
                sw.SpinOnce();
            }
            if ((fTCSQueue != null) && (fTCSQueue.Count > 0))
            {
                PatternFuture tcs = fTCSQueue.Dequeue();
                ret = DoReceive(tcs.Pattern);
                if (ret != null)
                {
                    tcs.Message.SetResult(ret);
                }
                else
                {
                    fTCSQueue.Enqueue(tcs);
                }
            }
            Interlocked.Exchange(ref fTaskReserved, 0);
            return ret != null;
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
                lTCS.Message = new TaskCompletionSource<Object>();
                if (fTCSQueue == null)
                {
                    fTCSQueue = new Queue<PatternFuture>();
                }
                fTCSQueue.Enqueue(lTCS);
                Interlocked.Exchange(ref fTaskReserved, 0);
                TrySetInTask(null);
                ret = await lTCS.Message.Task;
            }
            Interlocked.Decrement(ref fReceiveMode);
            return ret;
        }

        private Object DoReceive(Func<Object, bool> aPattern)
        {
            while (Interlocked.CompareExchange(ref messCount, 0, 0) != 0)
            {
                object msg = ReceiveMessage();
                if (msg != null) // no more message ?
                {
                    if (aPattern(msg))
                    {
                        return msg;
                    }
                    // no pattern apply, this message is postpone in receive mode
                    else
                    {
                        fMailBox.AddReceiveMiss(msg);
                    }
                }
                else
                {
                    Console.WriteLine("receive no mess");
                }
            }

            return null;
        }

        internal void Run()
        {
            AddReceiveMissedMessages();
            if (Interlocked.CompareExchange(ref fReceiveMode, 0, 0) != 0)
            {
                AddRunMissedMessages();
            }
            Stopwatch sw = Stopwatch.StartNew();
            int lPatternApplyer = 0;
            while (Interlocked.CompareExchange(ref messCount, 0, 0) != 0)
            {
                if (Interlocked.CompareExchange(ref fReceiveMode, 0, 0) != 0)
                {
                    AddRunMissedMessages();
                    if (TryReceive())
                    {
                        break;
                    }
                    AddReceiveMissedMessages();
                }
                Object msg = ReceiveMessage();
                if (msg != null) // no more message ?
                {
                    bool lPatternApply = false;
                    if (fBehaviors != null)
                    {
                        foreach (IBehavior Behavior in fBehaviors.GetBehaviors())
                        {
                            if (Behavior != null)
                            {
                                if (Behavior.StandardPattern(msg))
                                {
                                    lPatternApplyer++;
                                    lPatternApply = true;
                                    Behavior.StandardApply(msg);
                                    break; // at least one pattern apply
                                }
                            }
                        }
                    }
                    // no pattern apply, this message is missed
                    if (lPatternApply == false)
                    {
                        fMailBox.AddRunMiss(msg);
                        // break;
                    }
                    // at least one apply and we have hit the 20 ms barrier, better yielding
                    if ((lPatternApplyer > 0) && (sw.ElapsedMilliseconds >= 20))
                    {
                        AddRunMissedMessages();
                        break;
                    }
                }
                else break;
            }
            sw.Stop();

            Interlocked.Decrement(ref fInTask);

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
            AddRunMissedMessages();
            TrySetInTask(null);
        }

        protected void Become(IBehavior aBehavior)
        {
            fBehaviors = new Behaviors();
            fBehaviors.AddBehavior(aBehavior);
            fBehaviors.LinkToActor(this);
            AddRunMissedMessages();
            TrySetInTask(null);
        }

        protected void AddBehavior(IBehavior aBehavior)
        {
            fBehaviors.AddBehavior(aBehavior);
            AddRunMissedMessages();
            TrySetInTask(null);
        }

        protected void RemoveBehavior(IBehavior aBehavior)
        {
            AddRunMissedMessages();
            fBehaviors.RemoveBehavior(aBehavior);
        }

    }

}
