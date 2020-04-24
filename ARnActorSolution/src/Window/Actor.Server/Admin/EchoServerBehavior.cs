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

using System.Diagnostics;
using Actor.Base;

namespace Actor.Server
{
    public class EchoServerBehavior : ServerBehavior<string>
    {
        private const string MessageBhvEchoServer = "bhvEchoServer : null message received";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        protected override void DoRequest(ServerMessage<string> aMessage)
        {
            if (aMessage != null)
            {
                // echo to console
                SendByName<string>.Send(
                    "server receive " + aMessage.Data, "Console");
                // back to client but we need client
                if (aMessage.Client == null)
                {
                    Debug.WriteLine("receive null client");
                }
                SendAnswer(aMessage, aMessage.Data);
            }
            else
            {
                throw new ActorException(MessageBhvEchoServer);
            }
        }
    }
}
