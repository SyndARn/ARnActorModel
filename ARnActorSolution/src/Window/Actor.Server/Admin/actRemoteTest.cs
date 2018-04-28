using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actor.Base;

namespace Actor.Server
{
    // TODO to be completed
    internal class RemoteNetActor : BaseActor
    {
        public static void SendString(string aMsg)
        {
            var actor = new RemoteNetActor();
            var localhost = System.Net.Dns.GetHostName();

            //actor.SendMessage(aMsg,new actTag(new Uri("http://"+localhost+"/ARnActorServer/",UriKind.Absolute),"1")) ;
        }
    }
}
