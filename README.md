# ARnActorSolution
ARnActor Actor Model for C#

I was looking for an actor model things with some requirements :
 - no dependancy from another library
 - coding in plain C#, with such things like SendMessage, new Actor(), ...
 - 
 
With ARnActor, now you can :

   var sender = new actActor() ;
   var receiver = new actActor() ;
   and in sender code ..
   SendMessageTo("something",receiver) ;
   
#The library holds some useful feature :
-  behavior can be affected or remove from actor, (an actor can change it's behavior ...)
-  actor can send messages across servers, you just need to hold a reference to another actor on a server ...
-  some actor can behave as public services, or be supervised
