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
using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public enum CrudAction { Get, Set, Update, Delete} ;



    public class CrudBehavior<TKey,TValue> : Behavior<CrudMessage<TKey,TValue>>
    {
         private readonly Dictionary<TKey,TValue> _keyValue;

         public CrudBehavior()
            : base()
        {
            _keyValue = new Dictionary<TKey, TValue>();

            Pattern = o => true ;

            Apply = (o) =>
            {
                switch (o.Action)
                {
                    case CrudAction.Get: Get(o.Key, o.Sender); break;
                    case CrudAction.Set: Set(o.Key,o.Value); break;
                    case CrudAction.Update: Update(o.Key,o.Value); break;
                    case CrudAction.Delete: Delete(o.Key); break;
                }
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Valider les arguments de méthodes publiques", MessageId = "1")]
        public void Get(TKey key, IActor sender)
        {
            CheckArg.Actor(sender);
            sender.SendMessage(_keyValue[key]);
        }

        public void Set(TKey key, TValue value)
        {
            _keyValue[key] = value ;
        }

        public void Update(TKey key, TValue value)
        {
            _keyValue[key] = value;
        }

        public void Delete(TKey key)
        {
            _keyValue.Remove(key);
        }

    }
}
