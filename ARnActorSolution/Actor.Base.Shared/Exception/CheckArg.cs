using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Actor.Base
{
    public static class CheckArg
    {
        public static void Stream(Stream aStream)
        {
            if (aStream == null)
            {
                throw new ActorException("Stream can't be null");
            }
        }

        public static void Address(string anAddress)
        {
            if (string.IsNullOrEmpty(anAddress))
            {
                throw new ActorException("Address should be filled");
            }
        }

        public static void Behavior(IBehavior aBehavior)
        {
            if (aBehavior == null)
            {
                throw new ActorException("behavior can't be null");
            }
        }

        public static void BehaviorParam(params IBehavior[] someBehaviors)
        {
            if (someBehaviors == null)
            {
                if (someBehaviors == null) throw new ActorException("Null someBehavior");
            }
        }

        public static void Behaviors(Behaviors someBehaviors)
        {
            if (someBehaviors == null)
            {
                if (someBehaviors == null) throw new ActorException("Null someBehavior");
            }
        }

        public static void Actor(IActor anActor)
        {
            Actor(anActor, "actor can't be null");
        }

        public static void Actor(IActor anActor, string aMessage)
        {
            if (anActor == null)
            {
                throw new ActorException(aMessage);
            }
        }
    }
}
