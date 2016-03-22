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

namespace Actor.Base
{

    public static class SendByName<T>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static void Send(T aData, string anActor)
        {
            var act = new actSendByName<T>();
            act.SendMessage(Tuple.Create(anActor, aData));
        }
    }

    /// <summary>
    /// actSendByName
    ///   SendByName works together with Directory
    ///   It allows to send message to an actor only knowing his alias name in Directory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class actSendByName<T> : actActor
    {
        // The message to be send
        private T origMessage;

        public actSendByName()
        {
            Become(
                new bhvBehavior<Tuple<String, T>>(msg => { return msg is Tuple<string, T>; },
                    FindBehavior));
        }

        // FindBehavior to find the alias in directory
        private void FindBehavior(Tuple<String, T> msg)
        {
            // find in directory
            origMessage = msg.Item2;
            Become(new bhvBehavior<Tuple<actDirectory.DirectoryRequest, IActor>>(ask => { return ask is Tuple<actDirectory.DirectoryRequest, IActor>; },
                SendBehavior));
            actDirectory.GetDirectory().Find(this, msg.Item1);
        }

        // SendBehavior to send message to actor found in directory
        private void SendBehavior(Tuple<actDirectory.DirectoryRequest, IActor> ans)
        {
            if (ans.Item2 != null)
            {
                ans.Item2.SendMessage(origMessage);
            }
            Become(
                new bhvBehavior<Tuple<String, T>>(msg => { return msg is Tuple<string, T>; },
                    FindBehavior));
        }

    }

}
