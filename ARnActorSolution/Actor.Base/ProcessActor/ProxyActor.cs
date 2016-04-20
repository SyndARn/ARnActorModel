//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Reflection;

//namespace Actor.Base.ProcessActor
//{

//    interface ITest
//    {
//        string GetData();
//        void SetData(string value);
//    }

//    class test : ITest
//    {
//        private string fData;
//        public void SetData(string value) { fData = value; }
//        public string GetData() { return fData; }
//    }

//    class ProxyMessageAction
//    {
//        public Action Act { get; set; }
//    }

//    class ProxyMessageAction<T>
//    {
//        public Action<T> Act { get; set; }
//    }

//    class ProxyMessageFunc<T>
//    {
//        public Func<T> Func { get; set; }
//    }

//    class ProxyMessageFunc<T1,T2>
//    {
//        public Func<T1,T2> Func { get; set; }
//    }

//    class DestinationProxyActor : BaseActor
//    {
//        private test fTest;
//        public DestinationProxyActor(test aTest) : base()
//        {
//            fTest = aTest;
//            Become(new Behavior<ProxyMessage>(
//                t => t is ProxyMessage,
//                t =>
//                {
//                    var result = fTest.GetType().InvokeMember(
//                        t.MethodName, System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Default,
//                        null, fTest, args);
//                    // cast method with args / if return type cast future / 
//                }));
//        }
//        public ITest CastInterface()
//        {
//            return new ProxyTest(this);
//        }
//    }

//    class ProxyTest : ITest
//    {
//        DestinationProxyActor fActor;
//        public ProxyTest(DestinationProxyActor actor)
//        {
//            fActor = actor;
//        }
//        public string GetData()
//        {
//            var future = new Future<string>();
//            ParameterInfo[] args = typeof(ITest).GetMethod("GetData").GetParameters();
//            var returnType = typeof(string);
//            var msg = new ProxyMessage
//                (
//                "GetData",
//                args,
//                returnType,
//                future);
//            fActor.SendMessage(msg);
//            return future.Result();
//        }

//        public void SetData(string value)
//        {
//            var future = new Future<string>();
//            var args = new List<Tuple<string, Type>>();
//            args.Add(Tuple.Create("value", typeof(string)));
//            var returnType = typeof(string);
//            var msg = new ProxyMessage
//                (
//                "GetData",
//                args,
//                returnType,
//                future);
//            fActor.SendMessage(msg);
//        }
//    }

//}
