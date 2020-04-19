using System.Collections.Generic;
using Actor.Base;

namespace Actor.Server
{
    public interface IActorServerCommand : IBehavior
    {
        string Key { get; }
    }
}