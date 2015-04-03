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
    enum ProxyCommand { RelayAsk, RelayFound } ;

    /// <summary>
    /// actProxy
    ///   Acts as a proxy between an internal actor, and an external actor known only by is Tag
    ///   Only proxy are exported to other servers
    /// </summary>
    class actProxy : actActor
    {
        private IActor fLocalActor;
        private Uri fConnectedUri;

        public actProxy()
        {
            var remoteBehavior = new bhvBehavior<Tuple<Uri, Object>>(t => { return true; }, RemoteBehavior);
            var localBehavior = new bhvBehavior<Tuple<IActor, Object>>(t => { return true; }, LocalBehavior);
            var behaviors = new Behaviors();
            behaviors.AddBehavior(remoteBehavior);
            behaviors.AddBehavior(localBehavior);
            BecomeMany(behaviors);
        }

        // we receive from URI, we send to actor
        private void RemoteBehavior(Tuple<Uri, Object> msg)
        {
            SendMessageTo(msg.Item2,fLocalActor);
        }

        // we receive from actor, we send to URI
        private void LocalBehavior(Tuple<IActor, Object> msg)
        {
            // find host relay directory
            var lTask = Receive(t => { return t is Tuple<actDirectory.DirectoryRequest, IActor>; })
                .ContinueWith(
                t =>
                {
                    var ask = t.Result as Tuple<actDirectory.DirectoryRequest, IActor>;
                    if (ask.Item2 != null)
                    {
                        // find host relay 
                        SendMessageTo(Tuple.Create(fConnectedUri, "Find"),ask.Item2);
                        var lTaskFindHost =
                            Receive(t2 => { return t2 is Tuple<Uri, IActor>; }).ContinueWith(
                            t2 =>
                            {
                                var ask2 = t2.Result as Tuple<Uri, IActor>;
                                if (ask2.Item2 != null)
                                {
                                    // send to host relay
                                    SendMessageTo(Tuple.Create(msg.Item2, fConnectedUri,ask2.Item2));
                                }
                            });
                    }
                });
        }

    }
}
