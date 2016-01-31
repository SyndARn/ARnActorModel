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

        internal bool fCancel = false;

        public actMessageLoop()
        {
        }

        public static void Start(actActor actor)
        {
            actMessageLoop loop = new actMessageLoop();
            loop.Loop(actor);
        }

        public void Loop(actActor actActor)
        {
            IBehavior tcs = null;
            Object msg = null;
            while ((!fCancel) && (Interlocked.CompareExchange(ref actActor.messCount, 0, 0) != 0))
            {
                // message             
                msg = actActor.ReceiveMessage();
                tcs = null;

                if (actActor.fBehaviors != null)
                {
                    tcs = actActor.fBehaviors.PatternMatching(msg);
                    if (tcs == null)
                    {
                        Queue<IBehavior> lQueue = new Queue<IBehavior>();
                        while (actActor.fCompletions.TryDequeue(out tcs))
                        {
                            if (!tcs.StandardPattern(msg))
                            {
                                lQueue.Enqueue(tcs);
                                tcs = null;
                            }
                            else
                            {
                                break;
                            }
                        }
                        while (lQueue.Count > 0)
                        {
                            actActor.fCompletions.Enqueue(lQueue.Dequeue());
                        }
                    }
                }

                if (tcs != null)
                {
                    if (tcs.StandardCompletion == null)
                    {
                        tcs.StandardApply(msg);
                        tcs = null;
                    } 
                    else
                    {
                        break ;
                    }
                }
                else
                {
                    if (msg != null)
                      actActor.fMailBox.AddMiss(msg);
                }
            }
            fCancel = true;
            Interlocked.Exchange(ref actActor.fInTask, 0);
            if ((tcs != null) && (tcs.StandardCompletion != null))
            {
                tcs.StandardCompletion.SetResult(msg);
            }
            else
            {
                if (Interlocked.CompareExchange(ref actActor.messCount, 0, 0) != 0)
                {
                    actActor.TrySetInTask(null);
                }
            }

        }

    }
}
