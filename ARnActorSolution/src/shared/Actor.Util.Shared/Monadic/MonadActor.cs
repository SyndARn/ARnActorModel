using System;
using Actor.Base;

namespace Actor.Util
{
    /*
     * type T
     * unit
     * bind
     * */
    public class ModadActor : BaseActor
    {
    }

    public class Identity<T>
    {
        public T Value { get; private set; }
        public Identity(T aValue) => Value = aValue;
    }

    public static class LinqHelper
    {
        public static Identity<C> SelectMany<A, B, C>(this Identity<A> a, Func<A, Identity<B>> fct, Func<A, B, C> select) => select(a.Value, a.Bind(fct).Value).ToIdentity();
    }

    public static class IdentityHelper
    {
        private const string MessageFuncCantBeNull = "Func can't be null";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static Identity<B> Bind<A, B>(this Identity<A> a, Func<A, Identity<B>> func)
        {
            if (func == null)
            {
                throw new ActorException(MessageFuncCantBeNull);
            }

            return func(a.Value);
        }

        public static Identity<T> ToIdentity<T>(this T value) => new Identity<T>(value);
    }

    interface IMayBe<T> { }
}
