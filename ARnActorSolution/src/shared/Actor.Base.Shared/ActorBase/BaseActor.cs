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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace Actor.Base
{
    public class BaseActor : IActor
    {
        public ActorTag Tag
        {
            get => _sharedStruct.fTag;
            private set => _sharedStruct.fTag = value;
        }

        private List<IBehavior> _behaviors = new List<IBehavior>(); // our behavior
        private IMessageQueue<IBehavior> _awaitingBehaviors = QueueFactory<IBehavior>.Current.GetQueue(); // receive behaviors
        private IActorMailBox<object> _mailBox = new ActorMailBox<object>(); // our mailbox
        private SharingStruct _sharedStruct = new SharingStruct();
        public IMessageTracerService MessageTracerService { get; set; }

        public static void CompleteInitialize(BaseActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor._behaviors = new List<IBehavior>();
            anActor._awaitingBehaviors = QueueFactory<IBehavior>.Current.GetQueue();
            anActor._mailBox = new ActorMailBox<object>();
            if (anActor.Tag != null)
            {
                return;
            }

            anActor.Tag = new ActorTag();
        }

        protected BaseActor(ActorTag previousTag) => Tag = previousTag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask(object msg)
        {
            if (msg != null)
            {
                _mailBox.AddMessage(msg);
            }

            TrySetInTask(TaskCreationOptions.None);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask(TaskCreationOptions taskCreationOptions)
        {
            if (Interlocked.CompareExchange(ref _sharedStruct.fInTask, 1, 0) != 0)
            {
                return;
            }

            ActorTask.AddActor(MessageLoop, taskCreationOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySetInTask()
        {
            if (Interlocked.CompareExchange(ref _sharedStruct.fInTask, 1, 0) != 0)
            {
                return;
            }

            ActorTask.AddActor(MessageLoop, TaskCreationOptions.None);
        }

        private void AddMissedMessages() => _mailBox.RefreshFromMissed();

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

        public static BaseActor operator +(BaseActor anActor, object aMessage) => Add(anActor, aMessage);

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

        public BaseActor() => Tag = new ActorTag();

        public async Task<object> ReceiveAsync(Func<object, bool> aPattern, int timeOutMS)
        {
            CheckArg.Pattern(aPattern);
            Behavior awaitingBehavior = new Behavior(aPattern, new TaskCompletionSource<object>());
            Interlocked.Increment(ref _sharedStruct.fReceive);
            _awaitingBehaviors.Add(awaitingBehavior);
            AddMissedMessages();
            Interlocked.Exchange(ref _sharedStruct.fInTask, 0);
            TrySetInTask(TaskCreationOptions.LongRunning);
            if (timeOutMS != Timeout.Infinite)
            {
                bool noTimeOut = awaitingBehavior.Completion.Task.Wait(timeOutMS);
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
                        {
                            lQueue.Enqueue(bhv);
                        }
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
                return await awaitingBehavior.Completion.Task.ConfigureAwait(false);
            }
        }

        public Task<object> ReceiveAsync(Func<object, bool> aPattern) => ReceiveAsync(aPattern, Timeout.Infinite);

        private bool ReceiveMessage(out object msg)
        {
            msg = _mailBox.GetMessage();
            return msg != null;
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

            _behaviors = manyBehaviors.ToList();
            Behaviors someBehaviors = new Behaviors(manyBehaviors);

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
            object msg = null;
            IBehavior behavior = null;
            int oldReceive = Interlocked.Exchange(ref _sharedStruct.fReceive, _sharedStruct.fReceive);
            while (ReceiveMessage(out msg))
            {
                behavior = MatchByPattern(msg);

                if (behavior != null)
                {
                    bool shouldBreak = Interlocked.CompareExchange(ref _sharedStruct.fReceive, _sharedStruct.fReceive, 0) != 0;
                    behavior.StandardApply(msg);
                    if (shouldBreak || Interlocked.CompareExchange(ref _sharedStruct.fReceive, _sharedStruct.fReceive, 0) != 0)
                    {
                        break;
                    }
                }
                else
                {
                    behavior = MatchByWaitingBehavior(msg);
                    if (behavior != null)
                    {
                        break;
                    }

                    _mailBox.AddMiss(msg);
                }
            }

            if (behavior?.AwaitingPattern != null)
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

        private IBehavior MatchByWaitingBehavior(object msg)
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

            if (lQueue == null)
            {
                return null;
            }

            while (lQueue.Count > 0)
            {
                _awaitingBehaviors.Add(lQueue.Dequeue());
            }

            return null;
        }
    }
}
