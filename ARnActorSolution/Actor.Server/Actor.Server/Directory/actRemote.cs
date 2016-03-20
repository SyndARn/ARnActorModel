﻿/*****************************************************************************
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    /// <summary>
    /// actRemoteActor
    ///   A remote Actor is used (transparently) when sending messages across servers (ie across process)
    /// </summary>
    public class actRemoteActor : actActor
    {

        private actTag fRemoteTag;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void CompleteInitialize(actRemoteActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor.Become(new bhvBehavior<Object>(anActor.DoRouting));
        }

        public actRemoteActor(actTag aTag)
            : base()
        {
            fRemoteTag = aTag;
            Become(new bhvBehavior<Object>(DoRouting));
        }

        private void DoRouting(Object aMsg)
        {
            SendRemoteMessage(aMsg);
        }

        private void SendRemoteMessage(Object aMsg)
        {
            // send message with http
            SerialObject so = new SerialObject();
            so.Data = aMsg;
            so.Tag = fRemoteTag;

            MemoryStream ms = new MemoryStream();
            try
            {
                NetDataActorSerializer.Serialize(so, ms);

                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    while (!sr.EndOfStream)
                        Debug.Print(sr.ReadLine());
                }

                ms.Seek(0, SeekOrigin.Begin);
                // No response expected
                using (var client = new HttpClient())
                {
                    using (var hc = new StreamContent(ms))
                    {
                        Uri uri = new Uri(so.Tag.Uri);
                        client.PostAsync(uri, hc).Wait();
                    }
                }
            }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                }
            }
        }
    }

}
