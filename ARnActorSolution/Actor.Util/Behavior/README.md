#####ARnActorModel

#####Actor.Util

#####Behavior

Here are regrouped some common behaviors, they can be used simply with an AddBehavior command inside your actors.

Take a look for example to the ForEach Behavior : 
  
  Adding this behavior to an actor 
  ````AddBehavior(new ForEachBehavior<string>()) ;````
    
  Then sending this message to this actor
  ````MyActor.SendMessage(MyStringList, MyActionString)````
    
  the ForEachBehavior will iterate through the StringList, create an actor for each item, and send a Tuple with the string and the action to run on it.
  
  Basically, the ForEach Behavior enables to apply an Action on an Enumerable with parallelism. Each actor spawns will be process in a concurrent way.
