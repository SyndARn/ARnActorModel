using System;
using System.Dynamic;
using System.Reflection;
using Actor.Base;

namespace Actor.TestApplication
{
    public class DynamicActor : DynamicObject, IActor
    {
        private class InternalDynamicActor : BaseActor
        {
            private dynamic _dynamic;

            public InternalDynamicActor(dynamic dyn) : base()
            {
                _dynamic = dyn;
                Become(new Behavior<Tuple<InvokeMemberBinder, object[]>>(DoDynamic));
            }

            public void DoDynamic(Tuple<InvokeMemberBinder, object[]> msg)
            {
                _dynamic.GetType().InvokeMember(
                    msg.Item1.Name,
                    BindingFlags.Public | BindingFlags.InvokeMethod, null, _dynamic,
                    msg.Item2);
            }
        }

        private IActor _actor;
        private readonly dynamic _dynamic;

        public DynamicActor(dynamic dynamic)
        {
            _dynamic = dynamic;
            _actor = new InternalDynamicActor(dynamic);
        }

        ActorTag IActor.Tag => _actor.Tag;

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            _actor.SendMessage(Tuple.Create(binder, args));
            result = null;
            return true;
        }

        void IActor.SendMessage(object msg) => _actor.SendMessage(Tuple.Create(msg));
    }

    public class TestDynActor
    {
        public TestDynActor()
        {
        }

        public void DoPrintSomething(string s) => Console.WriteLine("print " + s);
    }
}
