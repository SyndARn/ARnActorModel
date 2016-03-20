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

        public actMessageLoop()
        {
            fCancel = false;
        }

        public void Loop(actActor actActor)
        {
            Object msg = null;
            IBehavior receivetcs = null;
            bool patternmatch = false;
            bool receivematch = false;

            while ((!fCancel) && (Interlocked.CompareExchange(ref actActor.messCount, 0, 0) != 0))
            {
                // get message             
                msg = actActor.ReceiveMessage();
                patternmatch = false;
                receivematch = false;

                // pattern matching
                if (actActor.fBehaviors != null)
                {
                    IBehavior tcs = actActor.fBehaviors.PatternMatching(msg);
                    if (tcs != null)
                    {
                        tcs.StandardApply(msg);
                        patternmatch = true;
                    }
                }

                // receive pattern
                if (! patternmatch)
                {
                    Queue<IBehavior> lQueue = new Queue<IBehavior>();
                    while (actActor.fCompletions.TryDequeue(out receivetcs))
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
                        actActor.fCompletions.Enqueue(lQueue.Dequeue());
                    }
                }

                // miss
                if (!patternmatch && !receivematch && msg != null)
                {
                    actActor.fMailBox.AddMiss(msg);
                }
            }

            Interlocked.Exchange(ref actActor.fInTask, 0);
            if (receivematch)
            {
                receivetcs.StandardCompletion.SetResult(msg);
            }
                if (Interlocked.CompareExchange(ref actActor.messCount, 0, 0) != 0)
                {
                    actActor.TrySetInTask(null);
                }
        }

    }
}
