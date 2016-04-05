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
    public enum ObservableAction { Register, UnRegister} ;

    public class ObservableActor<T> : BaseActor
    {
        private CollectionActor<IActor> fCollection;

        public ObservableActor() : base()
        {
            fCollection = new CollectionActor<IActor>();
            Become(new Behavior<Tuple<ObservableAction, IActor>>(DoRegister));
            AddBehavior(new Behavior<T>(DoPublishData));
        }

        public void PublishData(T aT)
        {
            SendMessage(aT);
        }

        public void RegisterObserver(IActor anActor)
        {
            SendMessage(new Tuple<ObservableAction, IActor>(ObservableAction.Register, anActor));
        }

        public void UnRegisterObserver(IActor anActor)
        {
            SendMessage(new Tuple<ObservableAction, IActor>(ObservableAction.UnRegister, anActor));
        }

        private void DoRegister(Tuple<ObservableAction,IActor> msg)
        {
            if (msg.Item1.Equals(ObservableAction.Register))
            {
                fCollection.Add(msg.Item2);
            } else
            {
                fCollection.Remove(msg.Item2);
            }
        }

        private void DoPublishData(T aT)
        {
            var bct = new BroadCastActor<T>();
            bct.BroadCast(aT, fCollection);
        }

    }
}
