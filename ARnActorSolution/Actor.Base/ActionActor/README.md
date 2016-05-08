####ARnActorModel

#####Actor.Base

- ActionActor

An ActionActor is a customized Actor with an Action Behavior. It's mainly used to send a Lamba Action or a Delegate to be processed during the Actor Lifetime.

Sample usage : actor.SendMessage( () => DoSomething) ;

The DoSomething lambda will be executed inside the actor context.


