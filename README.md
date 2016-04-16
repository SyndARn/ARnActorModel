# ARnActorModel
A C# Actor Model library.

I was looking for an actor model things with some requirements :
 - no dependancy from another library
 - coding in plain C#, with such things like 'SendMessage', 'new Actor()', 'Become(behavior)'
 - actor can change behaviors
 - actor can send message
 - actor can create other actors
 - behaviors are dynamic
 - actor can send message across servers
 
#With ARnActor, now you can :

    var sender = new actActor() ;
    var receiver = new actActor() ;
 
 and in sender code ..
 
    receiver.SendMessage("something") ;
 
#The library holds some useful features :
-  behavior can be affected or remove from actor, (an actor can change it's behavior ...)
-  actor can send messages across servers, you just need to hold a reference to another actor on a server ...
-  some actor can behave as public services, or be supervised

Unit tests are included as all as some sample applications.
