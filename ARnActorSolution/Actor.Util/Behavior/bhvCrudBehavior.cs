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
    public enum CrudeAction { Get, Set, Update, Delete } ;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actCrudeActor<T> : BaseActor
    {
        public actCrudeActor()
            : base()
        {
            Become(new bhvCrudeBehavior<T>());
        }

        public T Get()
        {
            SendMessage(Tuple.Create(CrudeAction.Get, default(T)));
            var retVal = Receive(t => { return true; }).Result;
            return retVal == null ? default(T) : (T)retVal;
        }

        public void Insert(T aT)
        {
            SendMessage(Tuple.Create(CrudeAction.Set, aT));
        }

        public void Delete()
        {
            SendMessage(Tuple.Create(CrudeAction.Delete,default(T))) ;
        }

        public void Update()
        {
            SendMessage(Tuple.Create(CrudeAction.Update,default(T))) ;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvCrudeBehavior<T> : bhvBehavior<Tuple<CrudeAction, T>>
    {
         private T fValue;

         public bhvCrudeBehavior()
            : base()
        {
            fValue = default(T);
        }

        public void SelectValue(T msg)
        {
            fValue = msg;
        }

        public void InsertValue()
        {
            LinkedActor.SendMessage(fValue);
        }

        public void UpdateValue()
        {

        }

        public void DeleteValue()
        {
        }

    }
}
