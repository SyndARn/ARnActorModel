﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actor.Base;

namespace Actor.Server
{
    // TODO to be completed
    class actRemoteTest : BaseActor 
    {
        public static void SendString(string aMsg)
        {
            var actor = new actRemoteTest();
            var localhost = System.Net.Dns.GetHostName();

            //actor.SendMessage(aMsg,new actTag(new Uri("http://"+localhost+"/ARnActorServer/",UriKind.Absolute),"1")) ;
        }
    }
}