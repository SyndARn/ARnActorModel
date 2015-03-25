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
            if (fFullHost == "")
            {
                var localhost = Dns.GetHostName();
                var servername = ActorServer.GetInstance().Name;
                var prefix = "http://";
                var suffix = ":" + ActorServer.GetInstance().Port.ToString();
                fFullHost = prefix + localhost + suffix + "/" + servername + "/";
            }
            return fFullHost ;
        }
}

    [Serializable]
    public class actTag
    {
        private string fUri ;
        private bool fIsRemote;
        private long fId;

        public long Id { get { return fId; } }

        public string Uri
        {
            get {
                if (String.IsNullOrEmpty(fUri))
                {
                    fUri = actTagHelper.FullHost();
                }
                return fUri; }
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
            if (fUri == "")
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