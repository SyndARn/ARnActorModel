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
using Actor.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Actor.Util
{
    public class QueueActor<T> : ActionActor<T>
    {
        private readonly Queue<T> fQueue = new Queue<T>();

        public QueueActor()
            : base()
        {
        }

        public void Queue(T at) => SendAction(DoQueue, at);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<IMsgQueue<T>> TryDequeueAsync()
        {
            var retVal = AsyncReceive(t => t is IMsgQueue<T>);
            SendAction(DoDequeue);
            return await retVal as IMsgQueue<T>;
        }

        private void DoQueue(T at) => fQueue.Enqueue(at);

        private void DoDequeue()
        {
            if (fQueue.Count > 0)
            {
                this.SendMessage(new MsgQueue<T>(true, fQueue.Dequeue()));
            }
            else
            {
                this.SendMessage(new MsgQueue<T>(false, default(T)));
            }
        }
    }
}
