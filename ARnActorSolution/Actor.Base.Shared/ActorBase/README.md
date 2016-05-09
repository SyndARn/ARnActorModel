ARnActorModel

ActorBase

ActorBase regroups the base component of our model of actors :
	
	- IActor : an actor can send message to another actor if it has access to this interface
	
	- Mailbox : each actor has a mailbox, messages are sent from an actor to another's one mailbox, then messages are pull out from the box one at a time, and process one at a time.

	- Future : Future are helper actors, they handle the Reply part of the Send/Reply Pattern.
