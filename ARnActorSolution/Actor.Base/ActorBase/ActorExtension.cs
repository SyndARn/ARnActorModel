using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public static class IActorExtension
    {
        public static void SendMessage<T1>(this IActor anActor, T1 t1)
        {
            anActor.SendMessage(t1);
        }
        public static void SendMessage<T1, T2>(this IActor anActor, T1 t1, T2 t2)
        {
            anActor.SendMessage(new Tuple<T1, T2>(t1, t2));
        }
        public static void SendMessage<T1, T2, T3>(this IActor anActor, T1 t1, T2 t2, T3 t3)
        {
            anActor.SendMessage(new Tuple<T1, T2, T3>(t1, t2, t3));
        }
    }

    public static class BaseActorExtension
    {
        public static void tell(IActor anActor, object aT)
        {
            anActor.SendMessage(aT);
        }

        public static void SendMessage<T1>(this BaseActor anActor, T1 t1)
        {
            anActor.SendMessage(t1);
        }
        public static void SendMessage<T1, T2>(this BaseActor anActor, T1 t1, T2 t2)
        {
            anActor.SendMessage(new Tuple<T1, T2>(t1, t2));
        }
        public static void SendMessage<T1, T2, T3>(this BaseActor anActor, T1 t1, T2 t2, T3 t3)
        {
            anActor.SendMessage(new Tuple<T1, T2, T3>(t1, t2, t3));
        }
        public static async Task<Object> Receive<T>(this BaseActor anActor, Func<T, bool> aPattern)
        {
            return await anActor.Receive((o) =>
            {
                if (o is T)
                {
                    return aPattern((T)o);
                }
                else
                    return false;
            });
        }
        public static async Task<Object> Receive<T1, T2>(this BaseActor anActor, Func<T1, T2, bool> aPattern)
        {
            return await anActor.Receive((o) =>
            {
                Tuple<T1, T2> t = o as Tuple<T1, T2>;
                return t != null ? aPattern(t.Item1, t.Item2) : false;
            });
        }
        public static async Task<Object> Receive<T1, T2, T3>(this BaseActor anActor, Func<T1, T2, T3, bool> aPattern)
        {
            return await anActor.Receive((o) =>
            {
                Tuple<T1, T2, T3> t = o as Tuple<T1, T2, T3>;
                return t != null ? aPattern(t.Item1, t.Item2, t.Item3) : false;
            });
        }
    }

}
