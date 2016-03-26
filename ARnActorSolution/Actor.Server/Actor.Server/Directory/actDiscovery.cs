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
    /// Return public services in a shard
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actDiscovery : BaseActor
    {
        public actDiscovery(string hostAddress)
            : base()
        {
            Become(new bhvBehavior<string>(t => {return true ;},
                Disco)) ;
            SendMessage(hostAddress) ;
        }

        private void Disco(string hostAddress)
        {
            Become(new bhvBehavior<Dictionary<string,string>>(t => {return true ;}, 
                Found)) ;
            var rem = new actRemoteActor(new actTag(hostAddress));
            rem.SendMessage(new DiscoCommand(this));
        }

        private void Found(Dictionary<string,String> aList)
        {
            Console.WriteLine("Disco found:");
            foreach(string s in aList.Keys)
             Console.WriteLine(s + "-"+aList[s]);
            Become(null);
        }
    }

}
