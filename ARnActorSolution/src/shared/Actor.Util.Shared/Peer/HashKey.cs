using System;
using System.Text;
using System.Globalization;
#if !(NETFX_CORE || WINDOWS_UWP)
using System.Security.Cryptography;
#endif

namespace Actor.Util
{
public class HashKey : IComparable
{
#if NETFX_CORE || WINDOWS_UWP
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

        public override string ToString()
        {
            return fTab.ToString(); 
        }

#else
        private readonly byte[] fTab;
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
            byte[] tab;
            using (var sha1 = SHA1.Create())
            {
                tab = sha1.ComputeHash(Encoding.Unicode.GetBytes(key));
            }
            return new HashKey(tab);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in fTab)
            {
                sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

#endif
        public static int Compare(HashKey left, HashKey right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return 0;
            }
            if (left is null)
            {
                return -1;
            }
            return left.CompareTo(right);
        }

        public static bool operator ==(HashKey left, HashKey right)
        {
            if (left is null)
            {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(HashKey left, HashKey right)
        {
            return !(left == right);
        }
        public static bool operator <(HashKey left, HashKey right)
        {
            return (Compare(left, right) < 0);
        }
        public static bool operator >(HashKey left, HashKey right)
        {
            return (Compare(left, right) > 0);
        }

        public override bool Equals(object obj) => !(obj is HashKey other) ? false : this.CompareTo(other) == 0;

        public override int GetHashCode() => this.ToString().GetHashCode();
    }

}
