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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Actor.Base
{

    internal static class actTagHelper
    {

        private static long fBaseId = 0;

        internal static long CastNewTagId() { return Interlocked.Increment(ref fBaseId); }

        private static string fFullHost = "";

        internal static string FullHost()
        {
            if (string.IsNullOrEmpty(fFullHost))
            {
                var localhost = Dns.GetHostName();
                var servername = ActorServer.GetInstance().Name;
                var prefix = "http://";
                var suffix = ":" + ActorServer.GetInstance().Port.ToString();
                fFullHost = prefix + localhost + suffix + "/" + servername + "/";
            }
            return fFullHost;
        }
    }

    // [Serializable]
    public class actTag
    {
        private string fUri;
        private bool fIsRemote;
        private long fId;

        public long Id { get { return fId; } }

        public string Uri
        {
            get
            {
                if (String.IsNullOrEmpty(fUri))
                {
                    fUri = actTagHelper.FullHost();
                }
                return fUri;
            }
        }

        public bool IsRemote { get { return fIsRemote; } }

        public actTag()
        {
            fId = actTagHelper.CastNewTagId();
            fIsRemote = false;
        }

        public actTag(string lHostUri)
        {
            fId = actTagHelper.CastNewTagId();
            fUri = lHostUri;
            fIsRemote = true;
        }

        public string Key()
        {
            if (string.IsNullOrEmpty(fUri))
            {
                fUri = actTagHelper.FullHost();
            }
            if (IsRemote)
            {
                return Uri;
            }
            else
            {
                return Uri + Id.ToString();
            }
        }
    }
}