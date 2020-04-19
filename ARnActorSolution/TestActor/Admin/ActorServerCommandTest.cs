using System;
using Actor.Base;
using Actor.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestActor
{
    internal class ServiceActorTest : BaseActor
    {
        public ServiceActorTest(ServerCommandService service): base()
        {
            Become(service);
        }
    }

    [TestClass]
    public class ActorServerCommandTest
    {
        [TestMethod]
        public void DiscoServerCommandTest()
        {
            var service = new ServerCommandService();
            var command = new DiscoServerCommand();
            service.RegisterCommand(command);
            var actor = new ServiceActorTest(service);
            TestLauncherActor.Test(() => actor.SendMessage(command.Key, new string[] { }));
        }

        [TestMethod]
        public void StatServerCommandTest()
        {
            var service = new ServerCommandService();
            var command = new StatServerCommand();
            service.RegisterCommand(command);
            var actor = new ServiceActorTest(service);
            TestLauncherActor.Test(() => actor.SendMessage(command.Key, new string[] { }));
        }
    }
}
