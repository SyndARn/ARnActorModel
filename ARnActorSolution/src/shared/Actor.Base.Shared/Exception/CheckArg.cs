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
        private const string AddressShouldBeFilled = "Address should be filled";
        private const string StreamCantBeNull = "Stream can't be null";
        private const string BehaviorsCantBeNull = "behaviors can't be null";
        private const string BehaviorCantBeNull = "behavior can't be null";
        private const string NullBehaviors = "Null someBehaviors";
        private const string NullEnumerables = "Null enumerables";
        private const string UriCantBeNull = "Uri can't be null";
        private const string FutureMustExist = "Future must exist !";
        private const string NullPatternReceived = "Null pattern received";
        private const string ActorMustExist = "actor must exist";

        public static void Stream([ValidatedNotNull]Stream aStream)
        {
            if (aStream != null)
            {
                return;
            }

            throw new ActorException(StreamCantBeNull);
        }

        public static void Address([ValidatedNotNull]string anAddress)
        {
            if (!string.IsNullOrEmpty(anAddress))
            {
                return;
            }

            throw new ActorException(AddressShouldBeFilled);
        }

        public static void BehaviorEnumerable([ValidatedNotNull]IEnumerable<IBehavior> behaviors)
        {
            if (behaviors != null)
            {
                return;
            }

            throw new ActorException(BehaviorsCantBeNull);
        }

        public static void Behavior([ValidatedNotNull]IBehavior aBehavior)
        {
            if (aBehavior != null)
            {
                return;
            }

            throw new ActorException(BehaviorCantBeNull);
        }

        public static void BehaviorParam([ValidatedNotNull]params IBehavior[] someBehaviors)
        {
            if (someBehaviors != null || someBehaviors != null)
            {
                return;
            }

            throw new ActorException(NullBehaviors);
        }

        public static void Behaviors([ValidatedNotNull]Behaviors someBehaviors)
        {
            if (someBehaviors != null)
            {
                return;
            }

            throw new ActorException(NullBehaviors);
        }

        public static void Behaviors([ValidatedNotNull]IBehaviors someBehaviors)
        {
            if (someBehaviors != null)
            {
                return;
            }

            throw new ActorException(NullBehaviors);
        }

        public static void IEnumerable([ValidatedNotNull] IEnumerable enumerables)
        {
            if (enumerables != null)
            {
                return;
            }

            throw new ActorException(NullEnumerables);
        }

        public static void Uri([ValidatedNotNull] Uri anUri)
        {
            if (anUri != null)
            {
                return;
            }

            throw new ActorException(UriCantBeNull);
        }

        public static void Actor([ValidatedNotNull] IActor anActor)
        {
            if (anActor != null)
            {
                return;
            }

            throw new ActorException(ActorMustExist);
        }

        public static void Future([ValidatedNotNull] IFuture aFuture)
        {
            if (aFuture != null)
            {
                return;
            }

            throw new ActorException(FutureMustExist);
        }

        public static void Pattern([ValidatedNotNull]Func<object, bool> aPattern)
        {
            if (aPattern != null)
            {
                return;
            }

            throw new ActorException(NullPatternReceived);
        }
    }
}
