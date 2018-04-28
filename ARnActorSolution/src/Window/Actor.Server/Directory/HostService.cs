using System;
using System.Globalization;

namespace Actor.Server
{
    public class HostService : IHostService
    {
        public Uri GetHostUri(string name, int port)
        {
            const string prefix = "http://";
            var suffix = ":" + port.ToString(CultureInfo.InvariantCulture);
            var fullhost = prefix + name + suffix + "/" ;

            return new Uri(fullhost);
        }
    }
}
