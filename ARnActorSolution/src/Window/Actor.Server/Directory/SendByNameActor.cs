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

namespace Actor.Server
{
    /// <summary>
    /// SendByNameActor
    ///   SendByName works together with Directory
    ///   It allows to send message to an actor only knowing his alias name in Directory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SendByNameActor<T> : BaseActor
    {
        // The message to be send
        private T origMessage;

        public SendByNameActor() => Become(new Behavior<string, T>(FindBehavior));

        // FindBehavior to find the alias in directory
        private void FindBehavior(string msg1, T msg2)
        {
            // find in directory
            origMessage = msg2;
            Become(new Behavior<DirectoryActor.DirectoryRequest, IActor>(SendBehavior));
            DirectoryActor.GetDirectory().Find(this, msg1);
        }

        // SendBehavior to send message to actor found in directory
        private void SendBehavior(DirectoryActor.DirectoryRequest request, IActor actor)
        {
            actor?.SendMessage(origMessage);
            Become(new Behavior<string, T>(FindBehavior));
        }
    }
}
