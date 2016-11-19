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

    /// <summary>
    /// ActorMailBox
    /// </summary>
    public class ActorMailBox<T> : IActorMailBox<T>
    {
        private IMessageQueue<T> fQueue ; // all actors may push here, only this one may dequeue
        private IMessageQueue<T> fMissed ; // only this one use it in run mode
        
        public ActorMailBox()
        {
            fQueue = QueueFactory<T>.Cast();
            fMissed = QueueFactory<T>.Cast();
        }

        public bool IsEmpty
        {
            get { return fQueue.Count() == 0 ; }
        }

        public void AddMiss(T aMessage)
        {
            fMissed.Add(aMessage);
        }

        public int RefreshFromMissed()
        {
            int i = 0;
            T val = default(T);
            while (fMissed.TryTake(out val))
            {
                fQueue.Add(val) ;
                i++ ;
            }
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddMessage(T aMessage) => fQueue.Add(aMessage);

        public T GetMessage()
        {
            T val = default(T);
            fQueue.TryTake(out val) ;
            return val;
        }

    }

}
