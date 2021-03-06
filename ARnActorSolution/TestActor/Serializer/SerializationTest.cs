﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Actor.Base;
using Actor.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestActor
{
    internal class Test1 : ActionActor
    {
        public string Name { get; set; }
    }

    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void TestSerializeActor()
        {
            TestLauncherActor.Test(() =>
            {
                ActorServer.Start(ActorConfigManager.CastForTest());
                var tst1 = new Test1() { Name = "TestName1" };
                var tst2 = new Test1() { Name = "TestName2" };
                var lst = new List<IActor>
                {
                    tst1,
                    tst2
                };

                // serialize
                BaseActor actor = new BaseActor();
                SerialObject so = new SerialObject(lst, actor.Tag);
                NetDataContractSerializer dcs = new NetDataContractSerializer()
                {
                    SurrogateSelector = new ActorSurrogatorSelector(),
                    Binder = new ActorBinder()
                };
                using (MemoryStream ms = new MemoryStream())
                {
                    dcs.Serialize(ms, so);

                    ms.Seek(0, SeekOrigin.Begin);
                    object obj = dcs.ReadObject(ms);
                    SerialObject soread = (SerialObject)obj;

                    var lst2 = (List<IActor>)soread.Data;

                    Assert.AreEqual(2, lst2.Count);
                    var l1 = (RemoteSenderActor)(lst2[0]);
                    var l2 = (RemoteSenderActor)(lst2.Last());

                    Assert.AreEqual(so.Tag.Key(), soread.Tag.Key());

                    Assert.AreEqual(tst1.Tag.Key(), l1.fRemoteTag.Key());
                    Assert.AreEqual(tst2.Tag.Key(), l2.fRemoteTag.Key());
                }
            }, 30000);
        }
    }
}
