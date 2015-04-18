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
    public class actMonad : actActor
    {
    }

    public class monadIdentity<T> : actActor
    {
        public T Value {get; private set;}
        public monadIdentity(T aValue) {Value = aValue ;}
    }

    public static class monadIdentityHelper
    {
        public static monadIdentity<T> Unit<T>(this T aValue)
        {
            return new monadIdentity<T>(aValue) ;
        }
        public static monadIdentity<T> Bind<T>(this monadIdentity<T> aT, Func<T,monadIdentity<T>> fct)
        {
            return fct(aT.Value) ;
        }

        public static monadIdentity<T> ToIdentity<T>(this T aValue) 
        { return Unit(aValue); }

        public static monadIdentity<T> SelectMany<T>(this monadIdentity<T> aT, Func<T,monadIdentity<T>> fct)
        { return Bind(aT,fct); }
    }


}
