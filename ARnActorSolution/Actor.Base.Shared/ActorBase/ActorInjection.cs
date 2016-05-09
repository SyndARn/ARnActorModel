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
        BaseActor Actor { get; }
    }

    /// <summary>
    /// How to inject actor into another object
    /// Use Behaviours and and a public interface around
    /// </summary>

    public class ActorInjection : IRunningActor
    {
        BaseActor realActor = null;

        public static ActorInjection Cast(Behaviors bhvs)
        {
            ActorInjection inject = new ActorInjection();
            inject.realActor = new BaseActor(bhvs);
            return inject;
        }

        public BaseActor Actor
        {
            get { return realActor ; }
        }

        // now you can injection.actor.sendmessageto ...
    }

}
