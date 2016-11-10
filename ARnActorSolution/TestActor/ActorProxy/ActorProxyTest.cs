using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using Actor.Util;


namespace TestActor
{

    public interface IDataTest
    {
        void SetData(string aString);
        void AskData(IFuture<string> future);
    }

    public class DataTest : IDataTest
    {
        private string fData;
        public void SetData(string aString)
        {
            fData = aString;
        }
        public void AskData(IFuture<string> future)
        {
            CheckArg.Future(future);
            future.SendMessage(fData);
        }
    }


    [TestClass]
    public class ProxyTest
    {
        [TestMethod]
        public void DataProxyTest()
        {
            TestLauncherActor.Test(() =>
            {
                string data = "ARn was here !";
                IData actorTest = new DataProxy();
                actorTest.Store(data);
                string result = actorTest.Retrieve();
                Assert.AreEqual(data, result);
            });
        }

        [TestMethod]
        public void ActorProxyTest()
        {
            TestLauncherActor.Test(() =>
            {
                string data = "ARn was here !";
                IActorProxy actorTest = new ActorProxy();
                actorTest.Store(data);
                IFuture<string> result = actorTest.Retrieve();
                Assert.AreEqual(data, result.Result());
            });
        }

        [TestMethod]
        public void ProxyGeneratorTest()
        {
            TestLauncherActor.Test(() =>
            {
                string data = "ARn was here !";
                DataTest dataTest = new DataTest();
                IDataTest actorTest = ActorProxyGenerator<DataTest, IDataTest>.GenerateFacade(dataTest);
                actorTest.SetData(data);
                var future = new Future<string>();
                actorTest.AskData(future);
                Assert.AreEqual(data, future.Result());
            });
        }

    }
}
