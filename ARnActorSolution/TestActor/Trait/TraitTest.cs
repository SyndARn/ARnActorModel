using System;
using Actor.Base;
using Actor.Util; 
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestActor
{
    [TestClass]
    public class TraitTest
    {
        [TestMethod]
        public void ActorWithTraitTest()
        {
            TestLauncherActor.Test(() =>
                {

                    var actor = new ActorWithTrait<string>();
                    var trait = new Trait<string>();
                    actor.TraitService = trait;
                    actor.TraitService.SetData("data");
                }
                );
        }
    }
}
