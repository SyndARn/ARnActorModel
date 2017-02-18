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

    public class CrudActor<TKey,TValue> : BaseActor
    {
        public CrudActor()
            : base()
        {
            Become(new CrudBehavior<TKey,TValue>());
        }

        public Future<TValue> Get(TKey key)
        {
            var future = new Future<TValue>();
            this.SendMessage(new CrudMessage<TKey, TValue>(CrudAction.Get, key, default(TValue), (IActor)future));
            return future;
        }

        public void Set(TKey key, TValue value)
        {
            SendMessage(new CrudMessage<TKey,TValue>(CrudAction.Set, key,value, null));
        }

        public void Delete(TKey key)
        {
            SendMessage(new CrudMessage<TKey, TValue>(CrudAction.Delete,key,default(TValue),null)) ;
        }

        public void Update(TKey key, TValue value)
        {
            SendMessage(new CrudMessage<TKey, TValue>(CrudAction.Update,key, value, null)) ;
        }
    }

    public class CrudMessage<TKey, TValue>
    {
        public CrudMessage(CrudAction anAction, TKey aKey, TValue aValue, IActor sender)
        {
            Action = anAction;
            Key = aKey;
            Value = aValue;
            Sender = sender;
        }
        public CrudAction Action { get; private set; }
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public IActor Sender { get; set; }
    }

    public class CrudBehavior<TKey,TValue> : Behavior<CrudMessage<TKey,TValue>>
    {
         private Dictionary<TKey,TValue> fKV;

         public CrudBehavior()
            : base()
        {
            fKV = new Dictionary<TKey, TValue>();

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
            sender.SendMessage(fKV[key]);
        }

        public void Set(TKey key, TValue value)
        {
            fKV[key] = value ;
        }

        public void Update(TKey key, TValue value)
        {
            fKV[key] = value;
        }

        public void Delete(TKey key)
        {
            fKV.Remove(key);
        }

    }
}
