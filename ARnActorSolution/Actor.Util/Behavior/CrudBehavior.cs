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
    public enum CrudAction { Get, Set, Update, Delete } ;

    public class CrudActor<T> : BaseActor
    {
        public CrudActor()
            : base()
        {
            Become(new CrudBehavior<T>());
        }

        public T Get()
        {
            SendMessage(Tuple.Create(CrudAction.Get, default(T)));
            var retVal = Receive(t => { return true; }).Result;
            return retVal == null ? default(T) : (T)retVal;
        }

        public void Insert(T aT)
        {
            SendMessage(Tuple.Create(CrudAction.Set, aT));
        }

        public void Delete()
        {
            SendMessage(Tuple.Create(CrudAction.Delete,default(T))) ;
        }

        public void Update()
        {
            SendMessage(Tuple.Create(CrudAction.Update,default(T))) ;
        }
    }

    public class CrudBehavior<T> : Behavior<Tuple<CrudAction, T>>
    {
         private T fValue;

         public CrudBehavior()
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
