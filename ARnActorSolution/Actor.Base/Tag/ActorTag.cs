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

    public static class ActorTagHelper
    {

        private static long fBaseId = 0;

        internal static Guid CastNewTagId()
        {
            long baseId = Interlocked.Increment(ref fBaseId);
            Guid guid = new Guid(0, 0, 0, BitConverter.GetBytes(baseId));
            return guid;
        }

        internal static Guid CastWithHash(long hash)
        {
            long baseId = Interlocked.Increment(ref fBaseId);
            Guid guid = new Guid(BitConverter.GetBytes((hash << 64) + baseId));
            return guid;
        }

        private static string fFullHost = "";

        public static string GetFullHost() { return fFullHost;  }

        public static void SetFullHost(string aValue)
        {
            fFullHost = aValue;
        }

    }

    [Serializable]
    public class ActorTag
    {
        private string fUri;
        private bool fIsRemote;
        private Guid fId;

        public Guid Id { get { return fId; } }

        public string Uri
        {
            get
            {
                if (String.IsNullOrEmpty(fUri))
                {
                    fUri = ActorTagHelper.GetFullHost();
                }
                return fUri;
            }
        }

        private bool IsRemote { get { return fIsRemote; } }

        public ActorTag()
        {
            fId = ActorTagHelper.CastNewTagId();
            fIsRemote = false;
        }

        public ActorTag(string lHostUri)
        {
            fId = ActorTagHelper.CastNewTagId();
            fUri = lHostUri;
            fIsRemote = true;
        }

        public string Key()
        {
            if (string.IsNullOrEmpty(fUri))
            {
                fUri = ActorTagHelper.GetFullHost();
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