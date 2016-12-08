using System;
using System.Text;
#if !(NETFX_CORE || WINDOWS_UWP)
using System.Security.Cryptography;
#endif

namespace Actor.Util
{
#if NETFX_CORE || WINDOWS_UWP
    public class HashKey : IComparable
    {
        int fTab;
        public HashKey(int tab)
        {
            fTab = tab;
        }

        public static HashKey ComputeHash(string key)
        {
            return new HashKey(key.GetHashCode());
        }

        public int CompareTo(object obj)
        {
            return ((IComparable)fTab).CompareTo(obj);
        }
    }
#else
    public class HashKey : IComparable
    {
        byte[] fTab;
        public HashKey(byte[] tab)
        {
            fTab = tab;
        }

        public int CompareTo(object obj)
        {
            for(int i=0;i<fTab.Length; i++)
            {
                var r = fTab[i] - ((HashKey)obj).fTab[i];
                if (r != 0)
                    return r;
            }
            return 0;
        }

        public static HashKey ComputeHash(string key)
        {
            var sha1 = SHA1.Create();
            var tab = sha1.ComputeHash(Encoding.Unicode.GetBytes(key));
            return new HashKey(tab);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var b in fTab)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString(); 
        }
    }
#endif

}
