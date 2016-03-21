using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    class actRemoteTest : actActor 
    {
        public static void SendString(string aMsg)
        {
            var actor = new actRemoteTest();
            var localhost = System.Net.Dns.GetHostName();

            //actor.SendMessage(aMsg,new actTag(new Uri("http://"+localhost+"/ARnActorServer/",UriKind.Absolute),"1")) ;
        }
    }
}
