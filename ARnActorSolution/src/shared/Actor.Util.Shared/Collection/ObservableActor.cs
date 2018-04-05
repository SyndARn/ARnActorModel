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
using Actor.Base;

namespace Actor.Util
{
    public enum ObservableAction { Register, Unregister} ;

    public class ObservableActor<T> : BaseActor
    {
        private readonly List<IActor> fCollection;

        public ObservableActor() : base()
        {
            fCollection = new List<IActor>();
            Become(new Behavior<ObservableAction, IActor>(DoRegister));
            AddBehavior(new Behavior<T>(DoPublishData));
        }

        public void PublishData(T aT)
        {
            SendMessage(aT);
        }

        public void RegisterObserver(IActor anActor)
        {
            this.SendMessage(ObservableAction.Register, anActor);
        }

        public void UnRegisterObserver(IActor anActor)
        {
            this.SendMessage(ObservableAction.Unregister, anActor);
        }

        private void DoRegister(ObservableAction action, IActor actor)
        {
            if (action.Equals(ObservableAction.Register))
            {
                fCollection.Add(actor);
            } else
            {
                fCollection.Remove(actor);
            }
        }

        private void DoPublishData(T aT)
        {
            var bct = new BroadCastActor<T>();
            var currentObservers = fCollection.ToList(); // clone before sending 
            bct.BroadCast(aT, currentObservers);
        }
    }
}
