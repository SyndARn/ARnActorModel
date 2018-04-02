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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{

    /// <summary>
    /// actRemoteActor
    ///   A Remote Sender is used (transparently) when sending messages across servers (ie across process)
    /// </summary>
    public class RemoteSenderActor : BaseActor
    {

        private ISerializeService fSerializeService;

        // Don't touch !
        public ActorTag fRemoteTag;

        public static void CompleteInitialize(RemoteSenderActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor.fSerializeService = ActorServer.GetInstance().SerializeService;
            anActor.Become(new Behavior<Object>(anActor.DoRouting));
        }

        public RemoteSenderActor(ActorTag aTag)
            : base()
        {
            fRemoteTag = aTag;
            fSerializeService = ActorServer.GetInstance().SerializeService;
            Become(new Behavior<object>(DoRouting));
        }

        private void DoRouting(object aMsg)
        {
            SendRemoteMessage(aMsg);
        }

        private void SendRemoteMessage(object aMsg)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();

                fSerializeService.Serialize(aMsg,fRemoteTag, ms);                

                ms.Seek(0, SeekOrigin.Begin);

                IContextComm contextComm = ActorServer.GetInstance().ListenerService.GetCommunicationContext();
                contextComm.SendStream(fRemoteTag.Host,ms);

            }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                    ms = null;
                }
            }
        }
    }

}

