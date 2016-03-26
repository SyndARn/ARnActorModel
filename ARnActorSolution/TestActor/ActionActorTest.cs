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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actActionActorTest : ActionActor<string>
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
            var act = new ActionActor();
            act.SendAction(() =>
            {
                var tst = new actActionActorTest();
                tst.ConsoleWrite("bla");
            });
        }
    }
}
