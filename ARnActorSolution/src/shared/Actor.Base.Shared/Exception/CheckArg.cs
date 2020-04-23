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
        private const string MessageNullSomeBehaviors = "Null someBehaviors";
        private const string MessageBehaviorCantBeNull = "behavior can't be null";

        public static void Stream([ValidatedNotNull]Stream aStream)
        {
            if (aStream != null)
            {
                return;
            }

            throw new ActorException("Stream can't be null");
        }

        public static void Address([ValidatedNotNull]string anAddress)
        {
            if (!string.IsNullOrEmpty(anAddress))
            {
                return;
            }

            throw new ActorException("Address should be filled");
        }

        public static void BehaviorEnumerable([ValidatedNotNull]IEnumerable<IBehavior> behaviors)
        {
            if (behaviors != null)
            {
                return;
            }

            throw new ActorException("behaviors can't be null");
        }

        public static void Behavior([ValidatedNotNull]IBehavior aBehavior)
        {
            if (aBehavior != null)
            {
                return;
            }

            throw new ActorException(MessageBehaviorCantBeNull);
        }

        public static void BehaviorParam([ValidatedNotNull]params IBehavior[] someBehaviors)
        {
            if (someBehaviors != null || someBehaviors != null)
            {
                return;
            }

            throw new ActorException(MessageNullSomeBehaviors);
        }

        public static void Behaviors([ValidatedNotNull]Behaviors someBehaviors)
        {
            if (someBehaviors != null)
            {
                return;
            }

            throw new ActorException(MessageNullSomeBehaviors);
        }

        public static void Behaviors([ValidatedNotNull]IBehaviors someBehaviors)
        {
            if (someBehaviors != null)
            {
                return;
            }

            throw new ActorException(MessageNullSomeBehaviors);
        }

        public static void IEnumerable([ValidatedNotNull] IEnumerable enumerables)
        {
            if (enumerables != null)
            {
                return;
            }

            throw new ActorException("Null enumerables");
        }

        public static void Uri([ValidatedNotNull] Uri anUri)
        {
            if (anUri != null)
            {
                return;
            }

            throw new ActorException("Uri can't be null");
        }

        public static void Actor([ValidatedNotNull] IActor anActor)
        {
            if (anActor != null)
            {
                return;
            }

            throw new ActorException("actor must exist");
        }

        public static void Future([ValidatedNotNull] IFuture aFuture)
        {
            if (aFuture != null)
            {
                return;
            }

            throw new ActorException("future must exist !");
        }

        public static void Pattern([ValidatedNotNull]Func<object, bool> aPattern)
        {
            if (aPattern != null)
            {
                return;
            }

            throw new ActorException("Null pattern received");
        }

        //[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        //public sealed class ValidatedNotNullAttribute : Attribute
        //{
        //}
    }
}
