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
using System.Diagnostics;
using Actor.Base;

namespace Actor.Server
{
    public class RestSendBehavior : Behavior<Uri, IActor>
    {
        public RestSendBehavior()
            : base()
        {
            Pattern = DefaultPattern();
            Apply = DoRestPost;
        }

        private void DoRestPost(Uri uri, IActor actor)
        {
            (LinkedTo as BehaviorsRestReader).Answer = actor;
            var actWeb = new WebActor();
            var wr = WebActor.Cast(LinkedActor, uri);
            actWeb.SendMessage(wr);
        }
    }
}
