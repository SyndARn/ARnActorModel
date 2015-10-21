using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    /// <summary>
    /// How an actor is handled in running context
    /// </summary>
    public interface IRunningActor
    {
        actActor Actor { get; }
    }

    /// <summary>
    /// How to inject actor into another object
    /// Use Behaviours and and a public interface around
    /// </summary>

    public class ActorInjection : IRunningActor
    {
        actActor realActor = null;

        public static ActorInjection Cast(Behaviors bhvs)
        {
            ActorInjection inject = new ActorInjection();
            inject.realActor = new actActor(bhvs);
            return inject;
        }

        public actActor Actor
        {
            get { return realActor ; }
        }

        // now you can injection.actor.sendmessageto ...
    }

}
