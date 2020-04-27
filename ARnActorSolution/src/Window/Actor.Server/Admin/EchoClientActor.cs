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

using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public class EchoClientActor : BaseActor
    {
        private readonly EchoClientBehavior aClient;

        public EchoClientActor()
            : base()
        {
            aClient = new EchoClientBehavior();
        }

        public void Connect(string aServerName)
        {
            Become(new Behavior<string, string>((s1, s2) => s1 == "Connect", DoConnect));
            this.SendMessage("Connect", aServerName);
        }

        protected void DoConnect(string order, string host)
        {
            // find in directory
            DirectoryActor.GetDirectory().Find(this, host);
            ReceiveAsync(ask => (ask is IMessageParam<DirectoryActor.DirectoryRequest, IActor>)).ContinueWith(
                r =>
                {
                    IMessageParam<DirectoryActor.DirectoryRequest, IActor> ans = (IMessageParam<DirectoryActor.DirectoryRequest, IActor>)(r.Result);
                    if (ans.Item2 != null)
                    {
                        SendByName.Send("Server found", "Console");
                        ans.Item2.SendMessage(new ServerMessage<string>(this, ServerRequest.Connect, default));
                        ReceiveAsync(m =>
                            {
                                ServerMessage<string> sm = m as ServerMessage<string> ;
                                return m != null && (sm.Request.Equals(ServerRequest.Accept));
                            }).ContinueWith(
                            (c) =>
                            {
                                SendByName.Send("Client connected", "Console");
                                aClient.Connect(ans.Item2);
                                Become(aClient);
                            }, TaskScheduler.Default);
                    }
                    else
                    {
                        // Console.WriteLine("Retry");
                        this.SendMessage(order,host);
                        // Become(null);
                    }
                }, TaskScheduler.Default);
            // repeat message
        }

        public void SendMessage(string message)
        {
            SendMessage(new ServerMessage<string>(this, ServerRequest.Request, message));
        }
        //public void Disconnect()
        //{
        //    aClient.Disconnect();
        //}
    }
}
