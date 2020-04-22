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

namespace Actor.Base
{
    [Serializable]
    [DataContract]
    public class ActorTag : IEquatable<ActorTag>
    {
        [DataMember]
        private string _host;

        [DataMember]
        private bool _isRemote;

        [DataMember]
        private long _id;

        [DataMember]
        private int _uriHash;

        public string Host => _host;

        public ActorTag()
        {
            _id = ActorTagHelper.CastNewTagId();
            _host = ActorTagHelper.FullHost;
            _isRemote = false;
            _uriHash = (string.IsNullOrEmpty(_host)) ? 0 : _host.GetHashCode();
        }

        public ActorTag(string urlAddress)
        {
            CheckArg.Address(urlAddress);
            InitTag(new Uri(urlAddress));
        }

        public ActorTag(Uri uri)
        {
            CheckArg.Uri(uri);
            InitTag(uri);
        }

        private void InitTag(Uri uri)
        {
            _id = ActorTagHelper.CastNewTagId();
            _host = uri.AbsoluteUri;
            _isRemote = true;
            _uriHash = (string.IsNullOrEmpty(_host)) ? 0 : _host.GetHashCode();
        }

        public string Key() => string.Format(CultureInfo.InvariantCulture, "{0}-{1}", _uriHash, _id);

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

        public bool Equals(ActorTag other) => (other == null) ? false : _host == other._host && _isRemote == other._isRemote && _id == other._id;
    }
}