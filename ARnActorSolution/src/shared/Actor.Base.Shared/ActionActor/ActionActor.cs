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

namespace Actor.Base
{
    /// <summary>
    /// actActionActor
    ///     Action actor are a facility : they provide template to send method as message within an actor
    ///     e.g. SendMessage(() => {do something},anActor) ;
    /// </summary>
    public class ActionActor : BaseActor
    {
        public ActionActor() => Become(new ActionBehavior());

        public void SendAction(Action anAction) => SendMessage(anAction);
    }

    /// <summary>
    /// actActionActor
    ///   Action actor with the added type parameter if needed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActionActor<T> : BaseActor
    {
        public ActionActor() => Become(new ActionBehaviors<T>());

        public void SendAction(Action anAction) => SendMessage(anAction);

        public void SendAction(Action<T> anAction, T aT) => this.SendMessage(anAction, aT);
    }

    public class ActionActor<T1,T2> : BaseActor
    {
        public ActionActor() => Become(new ActionBehaviors<T1, T2>());

        public void SendAction(Action anAction) => SendMessage(anAction);

        public void SendAction(Action<T1,T2> anAction, T1 aT1, T2 aT2) => this.SendMessage(anAction, aT1, aT2);
    }
}
