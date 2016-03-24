using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Actor.Base
{
    internal class actMessageLoop
    {

        internal bool fCancel;
        actActor fActor;

        public actMessageLoop(actActor anActor)
        {
            fActor = anActor;
            fActor.AddMissedMessages();
            fCancel = false;
        }

        public void Loop()
        {
            Object msg = null;
            IBehavior receivetcs = null;
            bool patternmatch = false;
            bool receivematch = false;

            while ((!fCancel) && (Interlocked.CompareExchange(ref fActor.messCount, 0, 0) != 0))
            {
                // get message             
                msg = fActor.ReceiveMessage();
                if (msg != null)
                {
                    patternmatch = false;
                    receivematch = false;

                    // pattern matching
                    if (fActor.fBehaviors != null)
                    {
                        IBehavior tcs = fActor.fBehaviors.PatternMatching(msg);
                        if (tcs != null)
                        {
                            tcs.StandardApply(msg);
                            patternmatch = true;
                        }
                    }

                    // receive pattern
                    if (!patternmatch)
                    {
                        Queue<IBehavior> lQueue = new Queue<IBehavior>();
                        while (fActor.fCompletions.TryDequeue(out receivetcs))
                        {
                            if (!receivetcs.StandardPattern(msg))
                            {
                                lQueue.Enqueue(receivetcs);
                                receivetcs = null;
                            }
                            else
                            {
                                if (receivetcs.StandardCompletion != null)
                                {
                                    receivematch = true;
                                    fCancel = true;
                                    break;
                                }
                                else
                                    receivetcs = null;
                            }
                        }
                        while (lQueue.Count > 0)
                        {
                            fActor.fCompletions.Enqueue(lQueue.Dequeue());
                        }
                    }
                }
                // miss
                if (!patternmatch && !receivematch && msg != null)
                {
                    fActor.fMailBox.AddMiss(msg);
                }
            }

            Interlocked.Exchange(ref fActor.fInTask, 0);
            if (receivematch)
            {
                receivetcs.StandardCompletion.SetResult(msg);
                fCancel = true;
            }
            if (fCancel)
            {
                fActor.AddMissedMessages();
                fActor.TrySetInTask();
            }
            else
            {
                if (Interlocked.CompareExchange(ref fActor.messCount, 0, 0) != 0)
                {
                    fActor.TrySetInTask();
                }
            }
        }

    }
}
