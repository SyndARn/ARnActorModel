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
using System.Net.Sockets;
using System.Net;
using Actor.Base;

namespace Actor.Server
{
    public class ActorTcpServer : BaseActor
    {
        private readonly TcpListener _tcpListener;
        private readonly IPEndPoint _endPoint;

        public ActorTcpServer()
        {
            _endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            _tcpListener = new TcpListener(_endPoint);
            Become(new Behavior<string>(DoStartListen));
            _tcpListener.Start();
            SendMessage("Start Listen");
        }

        private void DoStartListen(string msg)
        {
            Task<TcpClient> client = _tcpListener.AcceptTcpClientAsync();
            IActor entryConnection = new ActorEntryConnection();
            entryConnection.SendMessage(client.Result);
            SendMessage("Continue Listen");
        }
    }
}
