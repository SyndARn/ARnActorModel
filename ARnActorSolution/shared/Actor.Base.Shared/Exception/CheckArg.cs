using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

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
                if (someBehaviors == null) throw new ActorException("Null someBehaviors");
            }
        }

        public static void Behaviors(Behaviors someBehaviors)
        {
            if (someBehaviors == null)
            {
                throw new ActorException("Null someBehaviors");
            }
        }

        public static void IEnumerable([ValidatedNotNullAttribute] IEnumerable enumerables)
        {
            if (enumerables == null)
            {
                throw new ActorException("Null enumerables");
            }
        }

        public static void Actor([ValidatedNotNullAttribute] IActor anActor)
        {
            if (anActor == null)
            {
                throw new ActorException("actor must exist");
            }
        }

        public static void Pattern(Func<object, bool> aPattern)
        {
            if (aPattern == null)
            {
                throw new ActorException("Null pattern received");
            }
        }

        sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
