using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;
using System.Collections.Generic;

namespace TestActor.ActorBase
{
    /*
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
        public static void SendMessage<T1, T2, T3, T4>(this IActor anActor, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            anActor.SendMessage(new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4));
        }
    }
        */


        class ExtensionTestActor : BaseActor
    {
        private List<string> fList = new List<string>();
        public ExtensionTestActor() : base()
        {
            Become(new Behavior<string>(s => fList.Add(s)));
            AddBehavior(new Behavior<string,string>((s1,s2) => fList.Add(s2)));
            AddBehavior(new Behavior<string, string,string>((s1, s2, s3) => fList.Add(s3)));
            AddBehavior(new Behavior<string, string, string, string>((s1, s2, s3, s4) => fList.Add(s4)));
            AddBehavior(new Behavior<IActor>(a => a.SendMessage(fList.AsReadOnly())));
        }
    }

    [TestClass]
    public class IActorExtensionTest
    {
        [TestMethod]
        public void TestActorExtension()
        {
            TestLauncherActor.Test(() =>
            {
                var receiver = new ExtensionTestActor();
                var future = new Future<Object>();
                var sender = new BaseActor();
                sender.SendMessage("test1");
                sender.SendMessage("test1","test2");
                sender.SendMessage("test1", "test2","test3");
                sender.SendMessage("test1", "test2", "test3", "test4");
                sender.SendMessage((IActor)future);
                var result = future.Result();
                Assert.IsNotNull(result);
                Assert.IsTrue(result is List<string>);
                Assert.AreEqual(4, ((List<string>)result).Count);
                Assert.AreEqual("test4", ((List<string>)result)[3]);
            });
        }
    }
}
