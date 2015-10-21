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

        internal int fCancel = 0;

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
            PatternFuture tcs = null;
            while ((fCancel ==0) && (Interlocked.CompareExchange(ref actActor.messCount, 0, 0) != 0))
            {
                Object msg = null;
                Action<Object> apply = null;

                // message             
                msg = actActor.ReceiveMessage();

                if (actActor.fBehaviors != null)
                {
                    IBehavior lBehavior = actActor.fBehaviors.PatternMatching(msg);
                    if (lBehavior != null)
                    {
                        apply = lBehavior.StandardApply;
                    }
                }

                if ((apply == null) && (Interlocked.CompareExchange(ref actActor.fReceiveMode, actActor.fReceiveMode, actActor.fReceiveMode) > 0))
                {
                    SpinWait spin = new SpinWait();
                    while (Interlocked.CompareExchange(ref actActor.fTaskReserved, 1, 0) != 0)
                    {
                        spin.SpinOnce();
                    }
                    Queue<PatternFuture> list = new Queue<PatternFuture>();
                    while ((tcs == null) && (actActor.fTCSQueue != null) && (actActor.fTCSQueue.Count > 0))
                    {
                        tcs = actActor.fTCSQueue.Dequeue();
                        if (tcs.Pattern(msg))
                        {
                            tcs.Message = msg;
                            break;
                        }
                        else
                        {
                            list.Enqueue(tcs);
                            tcs = null;
                        }
                    }
                    while (list.Any())
                    {
                        actActor.fTCSQueue.Enqueue(list.Dequeue());
                    }
                    Interlocked.Exchange(ref actActor.fTaskReserved, 0);
                }

                if (tcs != null)
                {
                    tcs.Message = msg;
                    break;
                }
                else
                    if (apply != null)
                    {
                        apply(msg);
                        apply = null;
                    }
                    else
                    {
                        actActor.fMailBox.AddMiss(msg);
                    }
            }

            fCancel = 1;
            Interlocked.Exchange(ref actActor.fInTask, 0);
            if (tcs != null)
            {
                tcs.TaskCompletion.SetResult(tcs.Message);
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
