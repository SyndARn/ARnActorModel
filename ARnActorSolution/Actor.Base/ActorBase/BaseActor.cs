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
        internal Behaviors fBehaviors; // our behavior
        internal ConcurrentQueue<IBehavior> fCompletions = new ConcurrentQueue<IBehavior>();
        internal ActorMailBox fMailBox = new ActorMailBox(); // our mailbox
        internal int fInTask = 0; // 0 out of task, 1 in task

        private IActor fRedirector = null;
        internal int messCount; // this should always be queue + postpone total
        bool fCancel = false;

        public bool IsRemote()
        {
            return Tag.IsRemote;
        }

        public static void CompleteInitialize(BaseActor anActor)
        {
            if (anActor == null) throw new ActorException("Null actor");
            anActor.fCompletions = new ConcurrentQueue<IBehavior>();
            anActor.fMailBox = new ActorMailBox();
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
            IActor aRedirector = new actRedirector(anActor);
            // this could be threaded ...
            Interlocked.Exchange(ref fRedirector, aRedirector);
        }

        private void AddMessage(Object aMessage)
        {
            fMailBox.AddMessage(aMessage);
            IncMess();
        }

        internal void TrySetInTask(Object msg)
        {
            if (msg != null)
            {
                AddMessage(msg);
            }
            TrySetInTask();
        }

        internal void TrySetInTask()
        {
            if (Interlocked.CompareExchange(ref fInTask, 1, 0) == 0)
            {
                ActorTask.AddActor(this);
            }
        }

        internal void AddMissedMessages()
        {
            // add all missed messages ...
            Interlocked.Add(ref messCount, fMailBox.RefreshFromMissed());
        }

        private static void DoSendMessageTo(Object msg, IActor aTargetActor)
        {
            ((BaseActor)aTargetActor).TrySetInTask(msg);
        }

        public void SendMessage(Object msg)
        {
            if (fRedirector != null)
            {
                DoSendMessageTo(new RedirectMessage(msg, this), fRedirector);
            }
            else
            {
                DoSendMessageTo(msg, this);
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

        protected async Task<Object> Receive(Func<Object, bool> aPattern)
        {
            if (aPattern == null)
                throw new ActorException("null pattern");
            Object ret = null;
            var lTCS = new Behavior<Object>(aPattern, new TaskCompletionSource<Object>());
            fCancel = true;
            fCompletions.Enqueue(lTCS);
            AddMissedMessages();
            Interlocked.Exchange(ref fInTask, 0);
            TrySetInTask();
            ret = await lTCS.Completion.Task;
            return ret;
        }

        private void IncMess()
        {
            Interlocked.Increment(ref messCount);
        }

        private void DecMess()
        {
            Interlocked.Decrement(ref messCount);
        }

        internal Object ReceiveMessage()
        {
            Object msg = fMailBox.GetMessage();
            if (msg != null)
            {
                DecMess();
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

        protected void Becomes(IBehavior[] manyBehaviors)
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


        internal void MessageLoop()
        {
            bool receivematch = false;
            Object msg = null;
            IBehavior tcs = null;            

            while (!fCancel)
            {
                bool patternmatch = false;
                while ((!patternmatch) && (!receivematch) && (Interlocked.CompareExchange(ref messCount, 0, 0) != 0))
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
                            Queue<IBehavior> lQueue = new Queue<IBehavior>();
                            while ((!fCancel) && (fCompletions.TryDequeue(out tcs)))
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
                                        receivematch = true;
                                        fCancel = true;
                                    }
                                    else
                                        tcs = null;
                                }
                            }
                            while (lQueue.Count > 0)
                            {
                                fCompletions.Enqueue(lQueue.Dequeue());
                            }
                        }
                    }

                    // miss
                    if (!patternmatch && !receivematch && msg != null )
                    {
                        fMailBox.AddMiss(msg);
                    }
                }
                if (patternmatch)
                {
                    tcs.StandardApply(msg);
                }
                else
                {
                    fCancel = true;
                }
            }

            Interlocked.Exchange(ref fInTask, 0);

            if (receivematch)
            {
                AddMissedMessages();
                tcs.StandardCompletion.SetResult(msg);
            }

            fCancel = false;

            if (Interlocked.CompareExchange(ref messCount, 0, 0) != 0)
            {
                TrySetInTask();
            }
        }
    }

}
