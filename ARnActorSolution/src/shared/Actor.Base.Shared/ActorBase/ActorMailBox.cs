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
using System.Runtime.CompilerServices;

namespace Actor.Base
{
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ActorMailBox<T> : IActorMailBox<T>
    {
        private readonly IMessageQueue<T> _queue; // all actors may push here, only this one may dequeue
        private readonly IMessageQueue<T> _missed; // only this one use it in run mode

        private static readonly QueueFactory<T> _factory = new QueueFactory<T>();

        public ActorMailBox()
        {
            _queue = _factory.GetQueue();
            _missed = _factory.GetQueue();
        }

        public bool IsEmpty => _queue.Count() == 0;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => ToString();

        public void AddMiss(T aMessage) => _missed.Add(aMessage);

        public int RefreshFromMissed()
        {
            int i = 0;
            while (_missed.TryTake(out T val))
            {
                _queue.Add(val);
                i++;
            }

            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddMessage(T aMessage) => _queue.Add(aMessage);

        public T GetMessage()
        {
            _queue.TryTake(out T val);
            return val;
        }
    }
}
