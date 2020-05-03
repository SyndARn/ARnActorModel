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
        private const string MessageBehaviorCantBeNull = "Behavior can't be null";
        private const string MessageNullEnumerables = "Null enumerables";
        private const string MessageUriCantBeNull = "Uri can't be null";
        private const string MessageAddressShouldBeFilled = "Address should be filled";
        private const string MessageFutureMustExist = "Future must exist !";
        private const string MessageStreamCantBeNull = "Stream can't be null";
        private const string MessageBehaviorsCantBeNull = "Behaviors can't be null";
        private const string MessageActorMustExit = "Actor must exist";
        private const string MessageNullPatternReceived = "Null pattern received";
        private const string MessageConfigManagerMustExist = "Config Manager must exist";
        private const string MessageNullObjectFound = "Null object encountered";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Stream([ValidatedNotNull]Stream aStream)
        {
            if (aStream != null)
            {
                return;
            }

            throw new ActorException(MessageStreamCantBeNull);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Address([ValidatedNotNull]string anAddress)
        {
            if (!string.IsNullOrEmpty(anAddress))
            {
                return;
            }

            throw new ActorException(MessageAddressShouldBeFilled);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void BehaviorEnumerable([ValidatedNotNull]IEnumerable<IBehavior> behaviors)
        {
            if (behaviors != null)
            {
                return;
            }

            throw new ActorException(MessageBehaviorsCantBeNull);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Behavior([ValidatedNotNull]IBehavior aBehavior)
        {
            if (aBehavior != null)
            {
                return;
            }

            throw new ActorException(MessageBehaviorCantBeNull);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void BehaviorParam([ValidatedNotNull]params IBehavior[] someBehaviors)
        {
            if (someBehaviors != null || someBehaviors != null)
            {
                return;
            }

            throw new ActorException(MessageNullSomeBehaviors);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Behaviors([ValidatedNotNull]Behaviors someBehaviors)
        {
            if (someBehaviors != null)
            {
                return;
            }

            throw new ActorException(MessageNullSomeBehaviors);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Behaviors([ValidatedNotNull]IBehaviors someBehaviors)
        {
            if (someBehaviors != null)
            {
                return;
            }

            throw new ActorException(MessageNullSomeBehaviors);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void IEnumerable([ValidatedNotNull] IEnumerable enumerables)
        {
            if (enumerables != null)
            {
                return;
            }

            throw new ActorException(MessageNullEnumerables);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Uri([ValidatedNotNull] Uri anUri)
        {
            if (anUri != null)
            {
                return;
            }

            throw new ActorException(MessageUriCantBeNull);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void ActorConfigManager([ValidatedNotNull] Object configManager)
        {
            if (configManager != null)
            {
                return;
            }

            throw new ActorException(MessageConfigManagerMustExist);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Actor([ValidatedNotNull] IActor anActor)
        {
            if (anActor != null)
            {
                return;
            }

            throw new ActorException(MessageActorMustExit);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Future([ValidatedNotNull] IFuture aFuture)
        {
            if (aFuture != null)
            {
                return;
            }

            throw new ActorException(MessageFutureMustExist);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Pattern([ValidatedNotNull]Func<object, bool> aPattern)
        {
            if (aPattern != null)
            {
                return;
            }

            throw new ActorException(MessageNullPatternReceived);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void IsNotNull([ValidatedNotNull]object anObj)
        {
            if (anObj != null)
            {
                return;
            }

            throw new ActorException(MessageNullObjectFound);
        }

        //[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
        //public sealed class ValidatedNotNullAttribute : Attribute
        //{
        //}
    }
}
