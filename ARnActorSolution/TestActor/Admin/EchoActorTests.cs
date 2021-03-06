﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Util;
using Actor.Base;

namespace Actor.Server.Tests
{
    internal class EchoTest : BaseActor
    {
        private string fData;

        public EchoTest() : base()
        {
            Become(new Behavior<IActor, string>((_, s) => fData = s));
            AddBehavior(new Behavior<IFuture<string>>((IFuture<string> a) => a.SendMessage(fData)));
        }
    }

    [TestClass()]
    public class EchoActorTests
    {
        [TestMethod()]
        public void EchoActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new EchoTest();
                EchoActor.Echo(actor, "Test Echo");
                IFuture<string> future = new Future<string>();
                actor.SendMessage(future);
                Assert.AreEqual("Test Echo", future.ResultAsync().Result);
            });
        }

        [TestMethod()]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1806:Ne pas ignorer les résultats des méthodes", Justification = "<En attente>")]
        public void EchoActorTest2()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new EchoTest();
                new EchoActor<string>(actor, "Test Echo");
                IFuture<string> future = new Future<string>();
                actor.SendMessage(future);
                Assert.AreEqual("Test Echo", future.ResultAsync().Result);
            });
        }
    }
}