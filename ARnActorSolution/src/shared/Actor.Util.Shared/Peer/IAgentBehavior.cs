using System;
using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public interface IAgentBehavior<TKey>
    {
        IFuture<IEnumerable<TKey>> AskKeys();
        IFuture<IEnumerable<IPeerActor<TKey>>> AskPeers();
    }

}
