namespace Actor.Base
{
    using System;
#if !NETFX_CORE
    using System.Runtime.Serialization;
#endif
    using System.Threading;

    public static class ActorTagHelper
    {
        [ThreadStatic]
        private static long fBaseId;

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

        public static string FullHost { get; set; } = "";
    }
}
