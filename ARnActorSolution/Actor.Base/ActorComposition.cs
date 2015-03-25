using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    // get two actors
    // f and g
    // create a new actor able to do f.g as composing actor
    class ActorComposition
    {
        public ActorComposition(IActor f, IActor g)
        {
            // every message send to this new actor
            // is send to f and from f to g
        }
    }
}
