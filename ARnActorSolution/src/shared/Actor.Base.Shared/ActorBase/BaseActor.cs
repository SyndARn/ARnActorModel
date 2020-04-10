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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace Actor.Base
{
    internal struct SharingStruct
    {
        public int fInTask; // 0 out of task, 1 in task
        public int fReceive;
        public ActorTag fTag;
#if DEBUG_MSG
        public int fMessCount; // this should always be queue + postpone total
#endif
    }

    public class BaseActor : IActor
    {
        public ActorTag Tag { get { return _sharedStruct.fTag; } private set { _sharedStruct.fTag = value; } } // unique identifier, and host

        private List<IBehavior> _behaviors = new List<IBehavior>(); // our behavior
        private static readonly QueueFactory<IBehavior> _factoryBehavior = new QueueFactory<IBehavior>();

        private IMessageQueue<IBehavior> _awaitingBehaviors = _factoryBehavior.GetQueue(); // receive behaviors
        private IActorMailBox<object> _mailBox = new ActorMailBox<object>(); // our mailbox
        private SharingStruct _sharedStruct = new SharingStruct();
        public IMessageTracerService MessageTracerService { get; set; }

        public static void CompleteInitialize(BaseActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor._behaviors = new List<IBehavior>();
            anActor._awaitingBehaviors = _factoryBehavior.GetQueue();
            anActor._mailBox = new ActorMailBox<object>();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask(Object msg)
        {
            if (msg != null)
            {
                _mailBox.AddMessage(msg);
#if DEBUG_MSG
                Interlocked.Increment(ref fShared.fMessCount);
#endif
            }
            TrySetInTask(TaskCreationOptions.None);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask(TaskCreationOptions taskCreationOptions)
        {
            if (Interlocked.CompareExchange(ref _sharedStruct.fInTask, 1, 0) == 0)
            {
                ActorTask.AddActor(MessageLoop, taskCreationOptions);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask()
        {
            if (Interlocked.CompareExchange(ref _sharedStruct.fInTask, 1, 0) == 0)
            {
                ActorTask.AddActor(MessageLoop, TaskCreationOptions.None);
            }
        }

        private void AddMissedMessages()
        {
            // add all missed messages ...
#if DEBUG_MSG
            Interlocked.Add(ref fShared.fMessCount, fMailBox.RefreshFromMissed());
#else
            _mailBox.RefreshFromMissed();
#endif

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SendMessage(object msg)
        {
            TrySetInTask(msg);
            MessageTracerService?.TraceMessage(msg);
            GlobalContext.MessageTracerService?.TraceMessage(msg);
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

        public BaseActor(IBehaviors someBehaviors)
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
            CheckArg.Pattern(aPattern);
            var awaitingBehavior = new Behavior(aPattern, new TaskCompletionSource<object>());
            Interlocked.Increment(ref _sharedStruct.fReceive);
            _awaitingBehaviors.Add(awaitingBehavior);
            AddMissedMessages();
            Interlocked.Exchange(ref _sharedStruct.fInTask, 0);
            TrySetInTask(TaskCreationOptions.LongRunning);
            if (timeOutMS != Timeout.Infinite)
            {
                var noTimeOut = awaitingBehavior.Completion.Task.Wait(timeOutMS);
                if (noTimeOut)
                {
                    return await awaitingBehavior.Completion.Task.ConfigureAwait(false);
                }
                else
                {
                    // remove this lTCS
                    Queue<IBehavior> lQueue = new Queue<IBehavior>();
                    while (_awaitingBehaviors.TryTake(out IBehavior bhv))
                    {
                        if (bhv != awaitingBehavior)
                            lQueue.Enqueue(bhv);
                    }
                    while (lQueue.Count > 0)
                    {
                        _awaitingBehaviors.Add(lQueue.Dequeue());
                    }

                    Interlocked.Decrement(ref _sharedStruct.fReceive);
                    return null;
                }
            }
            else
            {
                return await awaitingBehavior.Completion.Task;
            }
        }

        public Task<object> Receive(Func<object, bool> aPattern)
        {
            return Receive(aPattern, Timeout.Infinite);
        }

        private object ReceiveMessage()
        {
            object msg = _mailBox.GetMessage();
#if DEBUG_MSG
            if (msg != null)
            {
                Interlocked.Decrement(ref fShared.fMessCount);
            }
#endif
            return msg;
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

            var someBehaviors = new Behaviors();
            foreach (var item in manyBehaviors)
            {
                someBehaviors.AddBehavior(item);
                _behaviors.Add(item);
            }
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
            var behaviors = new Behaviors();

            foreach (var item in someBehaviors)
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
            bool matchedByWaitingBehavior = false;
            object msg = null;
            IBehavior behavior = null;
            bool matchedByPattern = false;
            int oldReceive = Interlocked.Exchange(ref _sharedStruct.fReceive, _sharedStruct.fReceive);
#if DEBUG_MSG
            while (Interlocked.CompareExchange(ref fShared.fMessCount, 0, 0) != 0)
#else
            do
#endif
            {
                // get message             
                msg = ReceiveMessage();
                if (msg != null)
                {
                    matchedByWaitingBehavior = false;

                    behavior = MatchByPattern(msg);
                    matchedByPattern = behavior != null;

                    // receive pattern
                    if (!matchedByPattern)
                    {
                        behavior = MatchByWaitingBehavior(msg);
                        matchedByWaitingBehavior = behavior != null;
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
                if (!matchedByPattern && !matchedByWaitingBehavior && msg != null)
                {
                    _mailBox.AddMiss(msg);
                }

                if (matchedByPattern)
                {
                    if (Interlocked.CompareExchange(ref _sharedStruct.fReceive, _sharedStruct.fReceive, 0) == 0)
                    {
                        behavior.StandardApply(msg);
                        matchedByPattern = false;
                        if (Interlocked.CompareExchange(ref _sharedStruct.fReceive, _sharedStruct.fReceive, 0) != 0)
                        {
                            break;
                        }
                    }
                }
            }
            while (!matchedByPattern && !matchedByWaitingBehavior);

            if (matchedByPattern)
            {
                behavior.StandardApply(msg);
            }

            if (matchedByWaitingBehavior)
            {
                AddMissedMessages();
                Interlocked.Decrement(ref _sharedStruct.fReceive);
                behavior.AwaitingPattern.SetResult(msg);
            }

            int newReceive = Interlocked.Exchange(ref _sharedStruct.fReceive, _sharedStruct.fReceive);

            if ((newReceive > 0) && (oldReceive != newReceive))
            {
                AddMissedMessages();
            }

            Interlocked.Exchange(ref _sharedStruct.fInTask, 0);

#if DEBUG_MSG
            if (Interlocked.CompareExchange(ref fShared.fMessCount, 0, 0) != 0)
#else
            if (!_mailBox.IsEmpty)
#endif
            {
                TrySetInTask();
            }
        }

        private IBehavior MatchByPattern(object msg)
        {
            foreach (var fBehavior in _behaviors)
            {
                if (fBehavior?.StandardPattern(msg) == true)
                {
                    return fBehavior;
                }
            }
            return null;
        }

        private IBehavior MatchByWaitingBehavior(Object msg)
        {
            Queue<IBehavior> lQueue = null;
            while (_awaitingBehaviors.TryTake(out IBehavior behavior))
            {
                if (lQueue == null)
                {
                    lQueue = new Queue<IBehavior>();
                }
                if (!behavior.StandardPattern(msg))
                {
                    lQueue.Enqueue(behavior);
                }
                else
                {
                    if (behavior.AwaitingPattern != null)
                    {
                        while (lQueue.Count > 0)
                        {
                            _awaitingBehaviors.Add(lQueue.Dequeue());
                        }
                        return behavior;
                    }
                }
            }
            if (lQueue != null)
            {
                while (lQueue.Count > 0)
                {
                    _awaitingBehaviors.Add(lQueue.Dequeue());
                }
            }
            return null;
        }
    }
}
