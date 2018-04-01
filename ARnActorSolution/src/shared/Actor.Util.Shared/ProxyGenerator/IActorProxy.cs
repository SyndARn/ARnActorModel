using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{ 

    public interface IActorProxy
    {
        void Store(string aData);
        IFuture<string> Retrieve();
    }

}
