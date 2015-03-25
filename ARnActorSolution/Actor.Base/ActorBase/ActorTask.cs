using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace Actor.Base
{

    public static class ActorTask
    {
        private static long numAddTask = 0; // qtt of task launched
        private static long numCloseTask = 0; // qtt of task finished, numAddTask - numCloseTask = 2 on actorserver at rest

        public static string Stat()
        {
            int workerThread;
            int ioThread;
            ThreadPool.GetAvailableThreads(out workerThread, out ioThread);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Max Active Threads " + workerThread.ToString() + " " + ioThread.ToString());
            sb.AppendLine("Task processed " + numAddTask.ToString());
            long total = numAddTask - numCloseTask; // 2 at rest, the actorserver AND the current task
            sb.AppendLine("Task running " + total.ToString());
            return sb.ToString();
        }

        public static void AddActor(actActor anActor)
        {
            if (anActor == null) throw new Exception("bad, no actor should be null at this point");
            Task.Factory.StartNew(() =>
            {
                Interlocked.Increment(ref numAddTask);
                anActor.Run();
                Interlocked.Increment(ref numCloseTask);
            }, TaskCreationOptions.None).ContinueWith((t) =>
            {
                Debug.WriteLine("task fault on {0}", t.Exception.InnerExceptions.ToString());
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}

