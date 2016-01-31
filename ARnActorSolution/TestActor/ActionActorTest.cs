using Actor.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Server;

namespace TestActor
{
    public class actActionActorTest : actAction<string>
    {
        public void ConsoleWrite(string aString)
        {
            SendAction(DoConsoleWrite, aString);
        }

        private void DoConsoleWrite(string aString)
        {
            Console.WriteLine("action receiver " + aString);
        }
    }

    [TestClass]
    public class ActionActorTest
    {
        [TestMethod]
        public void TestActionActor()
        {
            ActorServer.Start("test", 80);
            var act = new actActionActor();
            act.SendAction(() =>
            {
                var tst = new actActionActorTest();
                tst.ConsoleWrite("bla");
            });
        }
    }
}
