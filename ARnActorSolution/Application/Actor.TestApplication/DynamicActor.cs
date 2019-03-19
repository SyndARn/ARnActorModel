using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Dynamic;
using System.Reflection;
using System.Linq.Expressions;

namespace Actor.TestApplication
{
    public class DynamicActor : DynamicObject, IActor
    {

        private class InternalDynamicActor : BaseActor
        {
            private dynamic fDynamic;

            public InternalDynamicActor(dynamic dyn) : base()
            {
                fDynamic = dyn;
                Become(new Behavior<Tuple<InvokeMemberBinder, object[]>>(DoDynamic));
            }

            public void DoDynamic(Tuple<InvokeMemberBinder, object[]> msg)
            {
                fDynamic.GetType().InvokeMember(
                    msg.Item1.Name,
                    BindingFlags.Public | BindingFlags.InvokeMethod, null, fDynamic,
                    msg.Item2);
            }
        }

        private IActor fActor;
        private readonly dynamic fDynamic;

        public DynamicActor(dynamic dynamic)
        {
            fDynamic = dynamic;
            fActor = new InternalDynamicActor(dynamic);
        }

        ActorTag IActor.Tag
        {
            get
            {
                return fActor.Tag;
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
                fActor.SendMessage(Tuple.Create(binder, args));
                result = null;
                return true;
        }

        void IActor.SendMessage(object msg)
        {
            fActor.SendMessage(Tuple.Create(msg));
        }
    }

    public class TestDynActor
    {
        public TestDynActor()
        {
        }

        public void DoPrintSomething(string s)
        {
            Console.WriteLine("print " + s);
        }
    }
}
