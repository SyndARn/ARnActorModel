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
using System.Globalization;
#if !NETFX_CORE
using System.Runtime.Serialization;
#endif
using System.Threading;

namespace Actor.Base
{

    public static class ActorTagHelper
    {
        [ThreadStatic]
        private static long fBaseId ;

        internal static long CastNewTagId()
        {

#if !NETFX_CORE

            if (fBaseId == 0)
            {
                fBaseId = (long)Thread.CurrentThread.ManagedThreadId << 32;
            }
#endif
            return fBaseId++;
        }

        private static string fFullHost = "";

        public static string FullHost
        {
            get
            { return fFullHost; }
            set
            {
                fFullHost = value;
            }
        }

    }

    [Serializable]
    [DataContract]
    public class ActorTag : IEquatable<ActorTag>
    {
        [DataMember]
        private string fHost;
        [DataMember]
        private bool fIsRemote;
        [DataMember]
        private long fId;
        [DataMember]
        private int fUriHash;

        public string Host
        {
            get
            {
                return fHost;
            }
        }

        public ActorTag()
        {
            fId = ActorTagHelper.CastNewTagId();
            fHost = ActorTagHelper.FullHost;
            fIsRemote = false;
            fUriHash = string.IsNullOrEmpty(fHost) ? 0 : fHost.GetHashCode();
        }

        public ActorTag(string urlAddress)
        {
            fId = ActorTagHelper.CastNewTagId();
            fHost = urlAddress;
            fIsRemote = true;
            fUriHash = string.IsNullOrEmpty(fHost) ? 0 : fHost.GetHashCode();
        }

        public string Key()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", fUriHash, fId);
        }

        public override int GetHashCode()
        {
            return Key().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ActorTag other = obj as ActorTag;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(ActorTag other)
        {
            if (other == null) return false;
            return fHost == other.fHost && fIsRemote == other.fIsRemote && fId == other.fId;
        }
    }
}