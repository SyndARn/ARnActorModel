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
using Actor.Base;
using System.Globalization;

namespace Actor.Server
{
    /// <summary>
    /// Return public services in a shard
    /// </summary>
    public class DiscoveryActor : BaseActor
    {
        private const string DiscoFound = "Disco found:";

        public DiscoveryActor(string hostAddress)
            : base()
        {
            Become(new Behavior<string>(t => true,
                Disco)) ;
            SendMessage(hostAddress);
        }

        public DiscoveryActor(string hostAddress, IActor sender)
            : base()
        {
            Become(new Behavior<string,IActor>((t, actor) => true,
                Disco));
            this.SendMessage(hostAddress, sender);
        }

        private void Disco(string hostAddress, IActor actor)
        {
            Become(new Behavior<Dictionary<string, string>>(t => true,
                Found));
            var rem = new RemoteSenderActor(new ActorTag(hostAddress));
            rem.SendMessage(new DiscoCommand(actor));
        }

        private void Disco(string hostAddress)
        {
            Become(new Behavior<Dictionary<string,string>>(t => true ,Found)) ;
            var rem = new RemoteSenderActor(new ActorTag(hostAddress));
            rem.SendMessage(new DiscoCommand(this));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        private void Found(Dictionary<string,string> dico)
        {
            Console.WriteLine(DiscoFound);
            foreach (string s in dico.Keys)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,"{0} - {1}",s, dico[s]));
            }

            Become(new NullBehaviors());
        }
    }
}
