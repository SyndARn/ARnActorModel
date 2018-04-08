namespace Actor.Base
{
    public static class IActorExtension
    {
        public static void SendMessage<T1, T2>(this IActor anActor, T1 t1, T2 t2)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new MessageParam<T1, T2>(t1, t2));
        }

        public static void SendMessage<T1, T2, T3>(this IActor anActor, T1 t1, T2 t2, T3 t3)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new MessageParam<T1, T2, T3>(t1, t2, t3));
        }

        public static void SendMessage<T1, T2, T3, T4>(this IActor anActor, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(new MessageParam<T1, T2, T3, T4>(t1, t2, t3, t4));
        }
    }
}
