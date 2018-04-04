/*****************************************************************************
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

using System;
using System.Threading.Tasks;

namespace Actor.Base
{
    public static class BaseActorExtension
    {
        public static void SendMessage<T1, T2>(this BaseActor anActor, T1 t1, T2 t2)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new MessageParam<T1, T2>(t1, t2));
        }
        public static void SendMessage<T1, T2, T3>(this BaseActor anActor, T1 t1, T2 t2, T3 t3)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new MessageParam<T1, T2, T3>(t1, t2, t3));
        }
        public static void SendMessage<T1, T2, T3, T4>(this BaseActor anActor, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new MessageParam<T1, T2, T3, T4>(t1, t2, t3, t4));
        }
        public static async Task<object> Receive<T1, T2>(this BaseActor anActor, Func<T1, T2, bool> aPattern)
        {
            CheckArg.Actor(anActor);
            return await anActor.Receive((o) =>
            {
                IMessageParam<T1, T2> t = o as IMessageParam<T1, T2>;
                return t != null ? aPattern(t.Item1, t.Item2) : false;
            });
        }
        public static async Task<object> Receive<T1, T2, T3>(this BaseActor anActor, Func<T1, T2, T3, bool> aPattern)
        {
            CheckArg.Actor(anActor);
            return await anActor.Receive((o) =>
            {
                IMessageParam<T1, T2, T3> t = o as IMessageParam<T1, T2, T3>;
                return t != null ? aPattern(t.Item1, t.Item2, t.Item3) : false;
            });
        }
        public static async Task<object> Receive<T1, T2, T3, T4>(this BaseActor anActor, Func<T1, T2, T3, T4, bool> aPattern)
        {
            CheckArg.Actor(anActor);
            return await anActor.Receive((o) =>
            {
                IMessageParam<T1, T2, T3, T4> t = o as IMessageParam<T1, T2, T3, T4>;
                return t != null ? aPattern(t.Item1, t.Item2, t.Item3, t.Item4) : false;
            });
        }
    }
}
