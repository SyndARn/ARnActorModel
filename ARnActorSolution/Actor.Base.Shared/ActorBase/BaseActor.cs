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
// #define NO_FALSE_SHARING

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


[assembly: CLSCompliant(true)]
namespace Actor.Base
{

    public enum SystemMessage { NullBehavior };


#if NO_FALSE_SHARING
    [StructLayout(LayoutKind.Explicit,Size =128)]
    internal struct SharingStruct
    {
        // 16
        [FieldOffset(0)]
        long fPaddingBefore1;
        [FieldOffset(8)]
        long fPaddingBefore2;
        [FieldOffset(16)]
        long fPaddingBefore3;

        // 32
        [FieldOffset(20)]
        public int fInTask ; // 0 out of task, 1 in task
        [FieldOffset(24)]
        public int fReceive ;
        [FieldOffset(28)]
        public int fMessCount; // this should always be queue + postpone total
        [FieldOffset(32)]
        // public int fPadding;
        public ActorTag fTag;

        // 16
        [FieldOffset(40)]
        long fPaddingAfter1;
        [FieldOffset(48)]
        long fPaddingAfter2;
        [FieldOffset(56)]
        long fPaddingAfter3;
    }
#else
    internal struct SharingStruct
    {
        public int fInTask; // 0 out of task, 1 in task
        public int fReceive;
        public ActorTag fTag;
#if DEBUG_MSG
        public int fMessCount; // this should always be queue + postpone total
#endif
    }
#endif

    public class BaseActor : IActor
    {
        public ActorTag Tag { get { return fShared.fTag; } private set { fShared.fTag = value; } } // unique identifier, and host
        private List<IBehavior> fListBehaviors = new List<IBehavior>(); // our behavior
        private ConcurrentQueue<IBehavior> fCompletions = new ConcurrentQueue<IBehavior>(); // receive behaviors
        private ActorMailBox<object> fMailBox = new ActorMailBox<object>(); // our mailbox
        private IActor fRedirector = null;
        private SharingStruct fShared = new SharingStruct();


        public static void CompleteInitialize(BaseActor anActor)
        {
            if (anActor == null) throw new ActorException("Null actor");
            anActor.fListBehaviors = new List<IBehavior>();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask(Object msg)
        {
            if (msg != null)
            {
                fMailBox.AddMessage(msg);
#if DEBUG_MSG
                Interlocked.Increment(ref fShared.fMessCount);
#endif
            }
            TrySetInTask();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask()
        {
            if (Interlocked.CompareExchange(ref fShared.fInTask, 1, 0) == 0)
            {
                ActorTask.AddActor(MessageLoop);
            }
        }

        private void AddMissedMessages()
        {
            // add all missed messages ...
#if DEBUG_MSG
            Interlocked.Add(ref fShared.fMessCount, fMailBox.RefreshFromMissed());
#else
            fMailBox.RefreshFromMissed();
#endif

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SendWithRedirector(object msg, IActor aTargetActor)
        {
            ((BaseActor)aTargetActor).TrySetInTask(msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendMessage(object msg)
        {
            if (fRedirector != null)
            {
                SendWithRedirector(new RedirectMessage(msg, this), fRedirector);
            }
            else
            {
                TrySetInTask(msg);
            }
        }

        public static BaseActor Add(BaseActor anActor, object aMessage)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(aMessage);
            return anActor;
        }

        public static BaseActor operator +(BaseActor anActor, object aMessage)
        {
            return Add(anActor, aMessage);
        }

        public BaseActor(Behaviors someBehaviors)
        {
            Tag = new ActorTag();
            Become(someBehaviors);
        }

        public BaseActor(IBehavior aBehavior)
        {
            Tag = new ActorTag();
            Become(aBehavior);
        }

        public BaseActor(IBehavior[] someBehaviors)
        {
            Tag = new ActorTag();
            Become(someBehaviors);
        }

        public BaseActor()
        {
            Tag = new ActorTag();
        }

        public async Task<object> Receive(Func<object, bool> aPattern, int timeOutMS)
        {
            if (aPattern == null)
                throw new ActorException("null pattern");
            var lTCS = new Behavior<object>(aPattern, new TaskCompletionSource<object>());
            Interlocked.Increment(ref fShared.fReceive);
            fCompletions.Enqueue(lTCS);
            AddMissedMessages();
            Interlocked.Exchange(ref fShared.fInTask, 0);
            TrySetInTask();
            if (timeOutMS != Timeout.Infinite)
            {
                bool noTimeOut = lTCS.Completion.Task.Wait(timeOutMS);
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

                    Interlocked.Decrement(ref fShared.fReceive);
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
#if DEBUG_MSG
            if (msg != null)
            {
                Interlocked.Decrement(ref fShared.fMessCount);
            }
#endif
            return msg;
        }

        protected void Become(Behaviors someBehaviors)
        {
            CheckArg.Behaviors(someBehaviors);
            fListBehaviors.Clear();
            someBehaviors.LinkToActor(this);
            fListBehaviors.AddRange(someBehaviors.AllBehaviors());
            AddMissedMessages();
            TrySetInTask();
        }

        protected void Become(params IBehavior[] manyBehaviors)
        {
            if (manyBehaviors == null)
            {
                throw new ActorException("Null manyBehaviors");
            }

            fListBehaviors.Clear();

            var someBehaviors = new Behaviors();
            foreach (var item in manyBehaviors)
            {
                someBehaviors.AddBehavior(item);
                fListBehaviors.Add(item);
            }
            someBehaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void AddBehavior(Behaviors someBehaviors)
        {
            CheckArg.Behaviors(someBehaviors);
            someBehaviors.LinkToActor(this);
            fListBehaviors.AddRange(someBehaviors.AllBehaviors());
            AddMissedMessages();
            TrySetInTask();
        }

        protected void AddBehavior(params IBehavior[] someBehaviors)
        {
            CheckArg.BehaviorParam(someBehaviors);
            var behaviors = new Behaviors();

            foreach (var item in someBehaviors)
            {
                behaviors.AddBehavior(item);
                fListBehaviors.Add(item);
            }
            behaviors.LinkToActor(this);
            AddMissedMessages();
            TrySetInTask();
        }

        protected void RemoveBehavior(IBehavior aBehavior)
        {
            AddMissedMessages();
            fListBehaviors.Remove(aBehavior);
        }


        private void MessageLoop()
        {
            bool receivematch = false;
            object msg = null;
            IBehavior tcs = null;
            bool patternmatch = false;
            int oldReceive = Interlocked.Exchange(ref fShared.fReceive, fShared.fReceive);
#if DEBUG_MSG
            while (Interlocked.CompareExchange(ref fShared.fMessCount, 0, 0) != 0)
#else
            while (true)
#endif
            {

                // get message             
                msg = ReceiveMessage();
                if (msg != null)
                {
                    patternmatch = false;
                    receivematch = false;
                    tcs = null;

                    // pattern matching
                    foreach (var fBehavior in fListBehaviors)
                    {
                        if ((fBehavior != null) && (fBehavior.StandardPattern(msg)))
                        {
                            tcs = fBehavior;
                            patternmatch = true;
                            break;
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
#if DEBUG_MSG
#else
                else
                {
                    break;
                }
#endif
                // miss
                if (!patternmatch && !receivematch && msg != null)
                {
                    fMailBox.AddMiss(msg);
                }

                if (patternmatch)
                {
                    if (Interlocked.CompareExchange(ref fShared.fReceive, fShared.fReceive, 0) == 0)
                    {
                        tcs.StandardApply(msg);
                        patternmatch = false;
                        if (Interlocked.CompareExchange(ref fShared.fReceive, fShared.fReceive, 0) != 0)
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
                Interlocked.Decrement(ref fShared.fReceive);
                tcs.StandardCompletion.SetResult(msg);
            }

            int newReceive = Interlocked.Exchange(ref fShared.fReceive, fShared.fReceive);

            if ((newReceive > 0) && (oldReceive != newReceive))
            {
                AddMissedMessages();
            }

            Interlocked.Exchange(ref fShared.fInTask, 0);

#if DEBUG_MSG
            if (Interlocked.CompareExchange(ref fShared.fMessCount, 0, 0) != 0)
#else
            if (! fMailBox.IsEmpty)
#endif
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
