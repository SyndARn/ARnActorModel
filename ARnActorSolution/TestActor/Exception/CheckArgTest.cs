using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.IO;

namespace TestActor.ExceptionTest
{
    [TestClass]
    public class CheckArgTest
    {

        [TestMethod]
        public void CheckArgReturnTest()
        {
            using (var stream = new MemoryStream())
            {
                CheckArg.Stream(stream);
            }

            try
            {
                CheckArg.Stream(null);
                Assert.Fail();
            }
            catch(ActorException)
            {
            }

            CheckArg.Address("My address");
            try
            {
                CheckArg.Address(string.Empty);
                Assert.Fail();
            }
            catch(ActorException)
            {

            }

            CheckArg.Behaviors(new Actor.Base.Behaviors());
            try
            {
                CheckArg.Behaviors(null);
                Assert.Fail();
            }
            catch(ActorException)
            {

            }

            CheckArg.Behavior(new Actor.Base.Behavior<string>());
            try
            {
                CheckArg.Behavior(null);
                Assert.Fail();
            }
            catch(ActorException)
            {

            }

            CheckArg.Actor(new Actor.Base.BaseActor());
            try
            {
                CheckArg.Actor(null);
                Assert.Fail();
            }
            catch (ActorException)
            {

            }

        }
    }
}
