using System;
using Actor.Base;
using Actor.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestActor
{
    [TestClass]
    public class ActorServerCommandTest
    {
        [TestMethod]
        public void DiscoServerCommandTest()
        {
            var command = new DiscoServerCommand();
            try
            {
                command.Run();
            }
            catch(Exception e)
            {
                Assert.IsTrue(e is ActorException);
            }
            try
            {
                command.Run("Test");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ActorException);
            }
            command.Run("Test1", "Test2");
        }

        [TestMethod]
        public void StatServerCommandTest()
        {
            var command = new StatServerCommand();
            try
            {
                command.Run();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ActorException);
            }
            command.Run("Test");
        }
    }
}
