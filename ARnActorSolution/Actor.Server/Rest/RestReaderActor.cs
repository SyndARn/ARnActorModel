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
using System.Diagnostics;

namespace Actor.Server
{
    public class RestReaderActor : BaseActor
    {
        public RestReaderActor() : base()
        {
            Become(new BehaviorsRestReader()) ;
        }

        public void SendRest(Uri anUri, IActor answer) 
        {
            SendMessage(Tuple.Create(anUri, answer));
        }
    }

    public class BehaviorsRestReader : Behaviors
    {
        public IActor Answer { get; set; }
        public BehaviorsRestReader() : base()
        {
            this.AddBehavior(new RestSendBehavior()) ;
            this.AddBehavior(new RestReceiveBehavior()) ;
        }
    }

    public class RestReceiveBehavior : Behavior<WebAnswer>
    {
        public RestReceiveBehavior() : base()
        {
            Pattern = DefaultPattern();
            Apply = DoRestReceive;
        }
        private void DoRestReceive(WebAnswer webAnswer)
        {
            Debug.WriteLine("Receive {0}",webAnswer.Answer) ;
            var reader = this.LinkedTo as BehaviorsRestReader;
            reader.Answer.SendMessage(webAnswer.Answer);
        }
    }

    public class RestSendBehavior : Behavior<Uri,IActor>
    {
        public RestSendBehavior()
            : base()
        {
            Pattern = DefaultPattern();
            Apply = DoRestPost;
        }
        private void DoRestPost(Uri uri,IActor actor)
        {
            (LinkedTo as BehaviorsRestReader).Answer = actor;
            var actWeb = new WebActor();
            var wr = WebActor.Cast(LinkedActor, uri);
            actWeb.SendMessage(wr);
        }
    }
}
