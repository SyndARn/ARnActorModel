using System;

namespace Actor.Server
{
    public interface IHostService
    {
        Uri GetHostUri(string name, int port);
    }
}
