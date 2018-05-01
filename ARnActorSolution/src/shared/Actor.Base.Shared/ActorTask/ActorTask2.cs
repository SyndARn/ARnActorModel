﻿/*****************************************************************************
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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Actor.Base
{
    public static class ActorTaskLowTask
    {
        private static long numAddTask = 0; // qtt of task launched
        private static long numCloseTask = 0; // qtt of task finished, numAddTask - numCloseTask = 2 on actorserver at rest

        public static string Stat()
        {
#if ! NETFX_CORE && ! NETCOREAPP1_1
            ThreadPool.GetAvailableThreads(out int workerThread, out int ioThread);
#endif
            StringBuilder sb = new StringBuilder();
#if ! NETFX_CORE && ! NETCOREAPP1_1
            sb.Append("Max Active Threads ").Append(workerThread.ToString(CultureInfo.InvariantCulture)).Append(" ").AppendLine(ioThread.ToString(CultureInfo.InvariantCulture));
#endif
            sb.Append("Task processed ").AppendLine(numAddTask.ToString(CultureInfo.InvariantCulture));
            long total = numAddTask - numCloseTask; // 2 at rest, the actorserver AND the current task
            sb.Append("Task running ").AppendLine(total.ToString(CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        private static readonly ConcurrentBag<Action> actionQueue = new ConcurrentBag<Action>();
        private static readonly List<Task> taskQueue = new List<Task>();
        private static readonly Object Locked = new Object();
        private static readonly Lazy<Task> clockAwake = new Lazy<Task>(ClockFactory, true);
        private static long moved;

        private static Task ClockFactory()
        {
            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (Interlocked.CompareExchange(ref moved, 1, 1) == 1)
                    {
                        CastTask();
                    }
                    Task.Delay(1000 * 10).Wait();
                }
            });
        }

        private static void CastTask()
        {
            var task = new Task(
            () =>
            {
                Interlocked.Increment(ref numAddTask);
                while (actionQueue.TryTake(out Action msg))
                {
                    msg();
                }
                Interlocked.Increment(ref numCloseTask);
            });

            task.ContinueWith((t) =>
            {
                Interlocked.Exchange(ref moved, 0);
                lock (Locked)
                {
                    taskQueue.Remove(task);
                }
                if (t.IsFaulted)
                {
                    foreach (var item in t.Exception.InnerExceptions)
                    {
                        Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Task fault on {0}", item.Message), "[Task Actor Fault]");
                    }
                    throw t.Exception;
                }
            });
            lock (Locked)
            {
                taskQueue.Add(task);
                Interlocked.Exchange(ref moved, 1);
            }
            task.Start();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddActor(Action messageLoop, TaskCreationOptions taskCreationOptions)
        {
            if (messageLoop == null)
            {
                throw new ActorException(string.Format(CultureInfo.InvariantCulture, "bad, no actor should be null at this point {0}", nameof(messageLoop)));
            }

            if (!clockAwake.IsValueCreated)
            {
                if (clockAwake.Value == null)
                {
                    throw new ActorException(string.Format(CultureInfo.InvariantCulture, "bad, no task should be null at this point {0}", nameof(messageLoop)));
                }
            }

            if (taskCreationOptions == TaskCreationOptions.LongRunning)
            {
                var task = Task.Factory.StartNew(
                    () =>
                    {
                        Interlocked.Increment(ref numAddTask);
                        messageLoop();
                        Interlocked.Increment(ref numCloseTask);
                    }, taskCreationOptions)
                    .ContinueWith((t) =>
                    {
                        foreach (var item in t.Exception.InnerExceptions)
                        {
                            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Task fault on {0}", item.Message), "[Task Actor Fault]");
                        }
                        throw t.Exception;
                    },
                TaskContinuationOptions.OnlyOnFaulted);

                return;
            }

            actionQueue.Add(messageLoop);

            bool flag = false;
            lock (Locked)
            {
                flag = taskQueue.Count < 32;
            }
            if (flag)
            {
                CastTask();
            }
        }
    }
}


