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
    public enum CrudAction { Get, Set, Update, Delete} ;

    public class CrudActor<K,V> : BaseActor
    {
        public CrudActor()
            : base()
        {
            Become(new CrudBehavior<K,V>());
        }

        public Future<V> Get(K key)
        {
            var future = new Future<V>();
            this.SendMessage(new CrudMessage<K, V>(CrudAction.Get, key, default(V), (IActor)future));
            return future;
        }

        public void Set(K key, V value)
        {
            SendMessage(new CrudMessage<K,V>(CrudAction.Set, key,value, null));
        }

        public void Delete(K key)
        {
            SendMessage(new CrudMessage<K, V>(CrudAction.Delete,key,default(V),null)) ;
        }

        public void Update(K key, V value)
        {
            SendMessage(new CrudMessage<K, V>(CrudAction.Update,key, value, null)) ;
        }
    }

    public class CrudMessage<K, V>
    {
        public CrudMessage(CrudAction anAction, K aKey, V aValue, IActor sender)
        {
            Action = anAction;
            Key = aKey;
            Value = aValue;
            Sender = sender;
        }
        public CrudAction Action { get; private set; }
        public K Key { get; set; }
        public V Value { get; set; }
        public IActor Sender { get; set; }
    }

    public class CrudBehavior<K,V> : Behavior<CrudMessage<K,V>>
    {
         private Dictionary<K,V> fKV;

         public CrudBehavior()
            : base()
        {
            fKV = new Dictionary<K, V>();

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

        public void Get(K key, IActor sender)
        {
            sender.SendMessage(fKV[key]);
        }

        public void Set(K key, V value)
        {
            fKV[key] = value ;
        }

        public void Update(K key, V value)
        {
            fKV[key] = value;
        }

        public void Delete(K key)
        {
            fKV.Remove(key);
        }

    }
}
