using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public static class IActorExtension
    {
        public static void SendMessage<T1>(this IActor anActor, T1 t1)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(t1);
        }
        public static void SendMessage<T1, T2>(this IActor anActor, T1 t1, T2 t2)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new Tuple<T1, T2>(t1, t2));
        }
        public static void SendMessage<T1, T2, T3>(this IActor anActor, T1 t1, T2 t2, T3 t3)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new Tuple<T1, T2, T3>(t1, t2, t3));
        }
        public static void SendMessage<T1, T2, T3, T4>(this IActor anActor, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4));
        }
        public static void SendString(this IActor anActor, string aString)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(aString);
        }
        public static void SendString(this IActor anActor, string aString, object arg0)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(string.Format(CultureInfo.InvariantCulture, aString, arg0));
        }
        public static void SendString(this IActor anActor, string aString, object arg0, object arg1)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(string.Format(CultureInfo.InvariantCulture, aString, arg0, arg1));
        }
        public static void SendString(this IActor anActor, string aString, object[] args)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(string.Format(CultureInfo.InvariantCulture, aString, args));
        }
    }

    public static class BaseActorExtension
    {
        public static void SendMessage<T1>(this BaseActor anActor, T1 t1)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(t1);
        }
        public static void SendMessage<T1, T2>(this BaseActor anActor, T1 t1, T2 t2)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new Tuple<T1, T2>(t1, t2));
        }
        public static void SendMessage<T1, T2, T3>(this BaseActor anActor, T1 t1, T2 t2, T3 t3)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new Tuple<T1, T2, T3>(t1, t2, t3));
        }
        public static void SendMessage<T1, T2, T3, T4>(this BaseActor anActor, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4));
        }
        public static async Task<Object> Receive<T>(this BaseActor anActor, Func<T, bool> aPattern)
        {
            CheckArg.Actor(anActor);
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
            CheckArg.Actor(anActor);
            return await anActor.Receive((o) =>
            {
                Tuple<T1, T2> t = o as Tuple<T1, T2>;
                return t != null ? aPattern(t.Item1, t.Item2) : false;
            });
        }
        public static async Task<Object> Receive<T1, T2, T3>(this BaseActor anActor, Func<T1, T2, T3, bool> aPattern)
        {
            CheckArg.Actor(anActor);
            return await anActor.Receive((o) =>
            {
                Tuple<T1, T2, T3> t = o as Tuple<T1, T2, T3>;
                return t != null ? aPattern(t.Item1, t.Item2, t.Item3) : false;
            });
        }
        public static async Task<Object> Receive<T1, T2, T3, T4>(this BaseActor anActor, Func<T1, T2, T3, T4, bool> aPattern)
        {
            CheckArg.Actor(anActor);
            return await anActor.Receive((o) =>
            {
                Tuple<T1, T2, T3, T4> t = o as Tuple<T1, T2, T3, T4>;
                return t != null ? aPattern(t.Item1, t.Item2, t.Item3, t.Item4) : false;
            });
        }
    }

    public class TaskCompletionSource<T1, T2> : TaskCompletionSource<Tuple<T1, T2>>
    {
    }
    public class TaskCompletionSource<T1, T2, T3> : TaskCompletionSource<Tuple<T1, T2, T3>>
    {
    }
    public class TaskCompletionSource<T1, T2, T3, T4> : TaskCompletionSource<Tuple<T1, T2, T3, T4>>
    {
    }

}
