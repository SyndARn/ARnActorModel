﻿/*****************************************************************************
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
using Actor.Server;

namespace Actor.Util
{
    public class actEchoServer : actActor
    {
        public actEchoServer()
            : base()
        {
            actHostDirectory.Register(this);
            actDirectory.GetDirectory().Register(this, "EchoServer");
            Become(new bhvEchoServer());
        }
    }

    public class actEchoClient : actActor
    {
        private bhvEchoClient aClient;
        public actEchoClient()
            : base()
        {
            aClient = new bhvEchoClient();
        }

        public void Connect(string ServerName)
        {
            Become(new bhvBehavior<Tuple<string, string>>(t => { return t.Item1 == "Connect"; }, DoConnect));
            SendMessage(Tuple.Create("Connect", ServerName));
        }

        protected void DoConnect(Tuple<string, string> msgcon)
        {
            // find in directory
            actDirectory.GetDirectory().Find(this, msgcon.Item2);
            Receive(ask => { return (ask is Tuple<actDirectory.DirectoryRequest, IActor>); }).ContinueWith(
                r =>
                {
                    Tuple<actDirectory.DirectoryRequest, IActor> ans = (Tuple<actDirectory.DirectoryRequest, IActor>)(r.Result);
                    if (ans.Item2 != null)
                    {
                        actSendByName<string>.SendByName("Server found", "Console");
                        ans.Item2.SendMessage(new ServerMessage<string>(this, ServerRequest.Connect, default(string)));
                        Receive(m => (m is ServerMessage<string>) && (((ServerMessage<string>)m).Request.Equals(ServerRequest.Accept))).ContinueWith(
                            (c) =>
                            {
                                actSendByName<string>.SendByName("Client connected", "Console");
                                aClient.Connect(ans.Item2);
                                Become(aClient);
                            });
                    }
                    else
                    {
                        Console.WriteLine("Retry");
                        SendMessage(msgcon);
                        // Become(null);
                    }
                });
            // repeat message
        }

        public void SendMessage(string s)
        {
            SendMessage(new ServerMessage<string>(this, ServerRequest.Request, s));
        }
        //public void Disconnect()
        //{
        //    aClient.Disconnect();
        //}
    }

    class bhvEchoServer : bhvServer<string>
    {
        protected override void DoRequest(ServerMessage<string> aMessage)
        {
            if (aMessage != null)
            {
                // echo to console
                actSendByName<string>.SendByName(
                    "server receive " + aMessage.Data, "Console");
                // back to client but we need client
                if (aMessage.Client == null)
                {
                    Console.WriteLine("receive null client");
                }
                SendAnswer(aMessage, aMessage.Data);
            }
            else
            {
                throw new ActorException("bhvEchoServer : null message received") ;
            }
        }
    }


    class bhvEchoClient : bhvClient<string>
    {
        protected override void ReceiveAnswer(ServerMessage<string> aMessage)
        {
            // echo to console
            actSendByName<string>.SendByName(
                "client receive " + aMessage.Data, "Console");
        }
    }
}
