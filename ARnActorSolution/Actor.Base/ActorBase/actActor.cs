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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Actor.Base
{

    class actResender : actActor
    {
        public actResender() { }
        public void Resend(Object Message, IActor Target)
        {
            Become(new bhvBehavior<Tuple<Object, IActor>>(
                t => { return true; },
                t => Target.SendMessage(Message)));
        }
    }

    public enum SystemMessage { NullBehavior } ;
    // composing actor ...
    /// <summary>
    /// actor have a default target
    /// when use in composition
    /// this default target could be use to make relay
    /// message should transport this default target to enable the real sender
    /// message should handle sender,target,relaysender,relaytarget
    /// with relaysender and relaytarget, you could have a composition
    /// </summary>



    public class actActor : IActor//, IDisposable
    {
        public actTag Tag { get; private set; } // unique identifier, and host
        internal Behaviors fBehaviors; // our behavior
        internal ConcurrentQueue<IBehavior> fCompletions = new ConcurrentQueue<IBehavior>();
        internal ActorMailBox fMailBox = new ActorMailBox(); // our mailbox
        internal actMessageLoop currentLoop;
        internal int fInTask = 0; // 0 out of task, 1 in task

        private IActor fRedirector = null;
        internal int messCount; // this should always be queue + postpone total

        public bool IsRemote()
        {
            return Tag.IsRemote;
        }

        public static void CompleteInitialize(actActor anActor)
        {
            anActor.fCompletions = new ConcurrentQueue<IBehavior>();
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

        internal void TrySetInTask(Object msg)
        {
            if (msg != null)
            {
                AddMessage(msg);
            }
            if (Interlocked.CompareExchange(ref fInTask, 1, 0) == 0)
            {
                currentLoop = new actMessageLoop();
                ActorTask.AddActor(this);
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

        public static actActor Add(actActor anActor, Object aMessage)
        {
            anActor.SendMessage(aMessage);
            return anActor;
        }

        public static actActor operator +(actActor anActor, Object aMessage)
        {
            return Add(anActor, aMessage);
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

        public actActor(IBehavior[] someBehaviors)
        {
            Tag = new actTag();
            Becomes(someBehaviors);
        }

        public actActor()
        {
            Tag = new actTag();
        }

        protected async Task<Object> Receive(Func<Object, bool> aPattern)
        {
            Object ret = null;
            var lTCS = new bhvBehavior<Object>(aPattern,new TaskCompletionSource<Object>()) ;
            fCompletions.Enqueue(lTCS) ;
            AddMissedMessages();
            currentLoop.fCancel = true;
            Interlocked.Exchange(ref fInTask, 0);
            TrySetInTask(null);
            ret = await lTCS.Completion.Task;
            AddMissedMessages();
            TrySetInTask(null);
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

        protected void Becomes(IBehavior[] manyBehaviors)
        {
            fBehaviors = new Behaviors();
            foreach (var item in manyBehaviors)
            {
                fBehaviors.AddBehavior(item);
            }
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
