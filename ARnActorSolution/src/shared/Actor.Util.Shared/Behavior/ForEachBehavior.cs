﻿/*****************************************************************************
		               ARnActor Actor Model Library .Net
     
	 Copyright (C) {2016}  {ARn/SyndARn} 
 
 
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
using System;
using System.Collections.Generic;

namespace Actor.Util
{
    /// <summary>
    /// ForEachBehavior
    ///   Apply an Action on an IEnumerable by creating an actor for each item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ForEachBehavior<T> : Behavior<IEnumerable<T>, Action<T>>
    {
        public ForEachBehavior() : base()
        {
            Pattern = (e,a) => { return true; };
            Apply = ForEach;
        }

        private void ForEach(IEnumerable<T> list, Action<T> action)
        {
            foreach (T act in list)
            {
                new BaseActor(new DoForEachbehavior<T>()).SendMessage(act, action);
            }
        }
    }

    internal class DoForEachbehavior<T> : Behavior<T, Action<T>>
    {
        public DoForEachbehavior()
        {
            Pattern = (t,a) => { return true; };
            Apply = DoEach;
        }

        private void DoEach(T aT, Action<T> action) => action(aT);
    }
}
