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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

[assembly: CLSCompliant(true)]
namespace Actor.Base
{

    public enum SystemMessage { NullBehavior };
    // composing actor ...
    /// <summary>
    /// actor have a default target
    /// when use in composition
    /// this default target could be use to make relay
    /// message should transport this default target to enable the real sender
    /// message should handle sender,target,relaysender,relaytarget
    /// with relaysender and relaytarget, you could have a composition
    /// </summary>

    public class BaseActor : IActor
    {
        public ActorTag Tag { get; private set; } // unique identifier, and host

        private Behaviors fBehaviors; // our behavior
        private ConcurrentQueue<IBehavior> fCompletions = new ConcurrentQueue<IBehavior>(); // receive behaviors
        private ActorMailBox<object> fMailBox = new ActorMailBox<object>(); // our mailbox
        private int fInTask = 0; // 0 out of task, 1 in task
        private int fReceive = 0;

        private IActor fRedirector = null;
        private int messCount; // this should always be queue + postpone total

        public static void CompleteInitialize(BaseActor anActor)
        {
            if (anActor == null) throw new ActorException("Null actor");
            anActor.fCompletions = new ConcurrentQueue<IBehavior>();
            anActor.fMailBox = new ActorMailBox<object>();
            if (anActor.Tag == null)
            {
                anActor.Tag = new ActorTag();
            }
        }

        protected BaseActor(ActorTag previousTag)
            : base()
        {
            Tag = previousTag;
        }

        public void RedirectTo(IActor anActor)
        {
            IActor aRedirector = new RedirectorActor(anActor);
            // this could be threaded ...
            Interlocked.Exchange(ref fRedirector, aRedirector);
        }

        private void TrySetInTask(Object msg)
        {
            if (msg != null)
            {
                fMailBox.AddMessage(msg);
                Interlocked.Increment(ref messCount);
            }
            TrySetInTask();
        }

        private void TrySetInTask()
        {
            if (Interlocked.CompareExchange(ref fInTask, 1, 0) == 0)
            {
                ActorTask.AddActor(MessageLoop);
            }
        }

        private void AddMissedMessages()
        {
            // add all missed messages ...
            Interlocked.Add(ref messCount, fMailBox.RefreshFromMissed());
        }

        private static void DoSendMessageTo(Object msg, IActor aTargetActor)
        {
            ((BaseActor)aTargetActor).TrySetInTask(msg);
        }

        private void SendMessageTo(Object msg)
        {
            TrySetInTask(msg);
        }

        public void SendMessage(Object msg)
        {
            if (fRedirector != null)
            {
                DoSendMessageTo(new RedirectMessage(msg, this), fRedirector);
            }
            else
            {
                SendMessageTo(msg);
            }
        }

        public static BaseActor Add(BaseActor anActor, Object aMessage)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(aMessage);
            return anActor;
        }

        public static BaseActor operator +(BaseActor anActor, Object aMessage)
        {
            return Add(anActor, aMessage);
        }

        public BaseActor(Behaviors someBehaviors)
        {
            Tag = new ActorTag();
            BecomeMany(someBehaviors);
        }

        public BaseActor(IBehavior aBehavior)
        {
            Tag = new ActorTag();
            Become(aBehavior);
        }

        public BaseActor(IBehavior[] someBehaviors)
        {
            Tag = new ActorTag();
            Becomes(someBehaviors);
        }

        public BaseActor()
        {
            Tag = new ActorTag();
        }

        public async Task<object> Receive(Func<object, bool> aPattern, int TimeOutMs)
        {
            if (aPattern == null)
                throw new ActorException("null pattern");
            var lTCS = new Behavior<object>(aPattern, new TaskCompletionSource<object>());
            Interlocked.Increment(ref fReceive);
            fCompletions.Enqueue(lTCS);
            AddMissedMessages();
            Interlocked.Exchange(ref fInTask, 0);
            TrySetInTask();
            if (TimeOutMs != Timeout.Infinite)
            {
                bool noTimeOut = lTCS.Completion.Task.Wait(TimeOutMs);
                if (noTimeOut)
                {
                    return await lTCS.Completion.Task;
                }
                else
                {
                    // remove this lTCS
                    Queue<IBehavior> lQueue = new Queue<IBehavior>();
                    IBehavior bhv;
                    while (fCompletions.TryDequeue(out bhv))
                    {
                        if (bhv != lTCS)
                            lQueue.Enqueue(bhv);
                    }
                    while (lQueue.Count > 0)
                    {
                        fCompletions.Enqueue(lQueue.Dequeue());
                    }

                    Interlocked.Decrement(ref fReceive);
                    return null;
                }
            }
            else
            {
                return await lTCS.Completion.Task;
            }
        }

        public async Task<Object> Receive(Func<Object, bool> aPattern)
        {
            return await Receive(aPattern, Timeout.Infinite);
        }

        private Object ReceiveMessage()
        {
            Object msg = fMailBox.GetMessage();
            if (msg != null)
            {
                Interlocked.Decrement(ref messCount);
            }
            return msg;
        }

        protected void BecomeMany(Behaviors someBehaviors)
        {
            CheckArg.Behaviors(someBehaviors);
            fBehaviors = someBehaviors;
            fBehaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void Become(IBehavior aBehavior)
        {
            fBehaviors = new Behaviors();
            fBehaviors.AddBehavior(aBehavior);
            fBehaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void Becomes(params IBehavior[] manyBehaviors)
        {
            if (manyBehaviors == null) throw new ActorException("Null manyBehaviors");
            fBehaviors = new Behaviors();
            foreach (var item in manyBehaviors)
            {
                fBehaviors.AddBehavior(item);
            }
            fBehaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void AddBehaviors(IBehavior[] someBehavior)
        {
            foreach(var item in someBehavior)
              fBehaviors.AddBehavior(item);
            TrySetInTask();
        }

        protected void AddBehavior(IBehavior aBehavior)
        {
            fBehaviors.AddBehavior(aBehavior);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void RemoveBehavior(IBehavior aBehavior)
        {
            AddMissedMessages();
            fBehaviors.RemoveBehavior(aBehavior);
        }


        private void MessageLoop()
        {
            bool receivematch = false;
            object msg = null;
            IBehavior tcs = null;
            bool patternmatch = false;
            int oldReceive = Interlocked.Exchange(ref fReceive, fReceive);

            while (Interlocked.CompareExchange(ref messCount, 0, 0) != 0)
            {

                // get message             
                msg = ReceiveMessage();
                if (msg != null)
                {
                    patternmatch = false;
                    receivematch = false;

                    // pattern matching
                    if (fBehaviors != null)
                    {
                        tcs = fBehaviors.PatternMatching(msg);
                        if (tcs != null)
                        {
                            patternmatch = true;
                        }
                    }

                    // receive pattern
                    if (!patternmatch)
                    {
                        tcs = ReceiveMatching(msg);
                        if (tcs != null)
                        {
                            receivematch = true;
                        }
                    }
                }

                // miss
                if (!patternmatch && !receivematch && msg != null)
                {
                    fMailBox.AddMiss(msg);
                }

                if (patternmatch)
                {
                    if (Interlocked.CompareExchange(ref fReceive,fReceive,0) ==0)
                    {
                        tcs.StandardApply(msg);
                        patternmatch = false;
                        if (Interlocked.CompareExchange(ref fReceive, fReceive, 0) != 0)
                        { 
                            break;
                        }
                    }
                }

                if (patternmatch || receivematch)
                    break;
            }

            if (patternmatch)
            {
                tcs.StandardApply(msg);
            }

            if (receivematch)
            {
                AddMissedMessages();
                Interlocked.Decrement(ref fReceive);
                tcs.StandardCompletion.SetResult(msg);
            }

            int newReceive = Interlocked.Exchange(ref fReceive, fReceive);

            if ((newReceive > 0) && (oldReceive != newReceive))
            {
                AddMissedMessages();
            }

            Interlocked.Exchange(ref fInTask, 0);

            if (Interlocked.CompareExchange(ref messCount, 0, 0) != 0)
            {
                TrySetInTask();
            }
        }

        private IBehavior ReceiveMatching(Object msg)
        {
            IBehavior tcs;
            Queue<IBehavior> lQueue = new Queue<IBehavior>();
            while (fCompletions.TryDequeue(out tcs))
            {
                if (!tcs.StandardPattern(msg))
                {
                    lQueue.Enqueue(tcs);
                    tcs = null;
                }
                else
                {
                    if (tcs.StandardCompletion != null)
                    {
                        break;
                    }
                    else
                        tcs = null;
                }
            }
            while (lQueue.Count > 0)
            {
                fCompletions.Enqueue(lQueue.Dequeue());
            }
            return tcs;
        }

    }

}
