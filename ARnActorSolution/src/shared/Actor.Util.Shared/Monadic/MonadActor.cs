using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Identity(T aValue) { Value = aValue; }
    }

    public static class LinqHelper
    {
        public static Identity<C> SelectMany<A, B, C>(this Identity<A> a, Func<A, Identity<B>> fct, Func<A, B, C> select)
        {
            return select(a.Value, a.Bind(fct).Value).ToIdentity();
        }
    }

    public static class IdentityHelper
    {
        public static Identity<B> Bind<A, B>(this Identity<A> a, Func<A, Identity<B>> func)
        {
            if (func == null)
            {
                throw new ActorException("func can't be null");
            }
            return func(a.Value);
        }

        public static Identity<T> ToIdentity<T>(this T value)
        {
            return new Identity<T>(value);
        }
    }

    interface IMayBe<T> { }
}
