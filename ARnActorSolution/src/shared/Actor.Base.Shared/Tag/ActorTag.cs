#region license
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
#endregion
using System;
using System.Globalization;
#if !NETFX_CORE
using System.Runtime.Serialization;
#endif
using System.Threading;

namespace Actor.Base
{
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

        public string Host => fHost;

        public ActorTag()
        {
            fId = ActorTagHelper.CastNewTagId();
            fHost = ActorTagHelper.FullHost;
            fIsRemote = false;
            fUriHash = string.IsNullOrEmpty(fHost) ? 0 : fHost.GetHashCode();
        }

        public ActorTag(string urlAddress)
        {
            CheckArg.Address(urlAddress);
            InitTag(urlAddress);
        }

        public ActorTag(Uri uri)
        {
            CheckArg.Uri(uri);
            InitTag(uri.Host);
        }

        private void InitTag(string urlAddress)
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

        public override int GetHashCode() => Key().GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is ActorTag other))
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(ActorTag other)
        {
            return other == null ? false : fHost == other.fHost && fIsRemote == other.fIsRemote && fId == other.fId;
        }
    }
}