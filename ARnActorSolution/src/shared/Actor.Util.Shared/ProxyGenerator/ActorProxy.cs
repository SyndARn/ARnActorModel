using Actor.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Util
{
    // Actor Proxy
    public class ActorProxy : IActorProxy
    {
        private RealActor fActor;
        public ActorProxy() : base()
        {
            fActor = new RealActor();
        }
        public IFuture<string> Retrieve()
        {
            return fActor.Retrieve();
        }

        public void Store(string aData)
        {
            fActor.Store(aData);
        }
    }

    public static class ActorHelper
    {
        public static void Store(this IActor actor, string aData)
        {
            CheckArg.Actor(actor);
            actor.SendMessage(aData);
        }
        public static IFuture<string> Retrieve(this IActor actor)
        {
            CheckArg.Actor(actor);
            IFuture<string> future = new Future<string>();
            actor.SendMessage(future);
            return future;
        }
        public static string RetrieveSync(this IActor actor)
        {
            CheckArg.Actor(actor);
            IFuture<string> future = new Future<string>();
            actor.SendMessage(future);
            return future.Result();
        }
    }

    public class RealActor : BaseActor
    {
        private string fData;
        public RealActor() : base()
        {
            var behaviorStore = new Behavior<string>(t => fData = t);
            var behaviorRetrieve = new Behavior<IFuture<string>>(t => t.SendMessage(fData));
            Become(behaviorStore, behaviorRetrieve);
        }
    }
}
