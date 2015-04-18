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
                anActor.currentLoop.Loop(anActor);
                Interlocked.Increment(ref numCloseTask);
            },TaskCreationOptions.None)
            .ContinueWith((t) =>
            {
                Debug.WriteLine("task fault on {0}", t.Exception.InnerExceptions.ToString());
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}

