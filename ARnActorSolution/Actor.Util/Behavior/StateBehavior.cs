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

    public class StateFullActor<T> : BaseActor
    {
        public StateFullActor()
            : base()
        {
            Become(new StateBehavior<T>());
        }

        public void Set(T aT)
        {
            SendMessage(Tuple.Create(StateAction.Set, aT));
        }

        public T Get()
        {
            SendMessage(Tuple.Create(StateAction.Get, default(T)));
            var retVal = Receive(t => { return true; }).Result;
            return retVal == null ? default(T) : (T)retVal;
        }
    }

    public enum StateAction { Set, Get } ;

    public class StateBehavior<T> : Behavior<Tuple<StateAction, T>>
    {
        private T fValue;

        public StateBehavior()
            : base()
        {
            fValue = default(T);
        }

        public void SetValue(T msg)
        {
            fValue = msg;
        }

        public void GetValue()
        {
            LinkedActor.SendMessage(fValue);
        }
    }

}
