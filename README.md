#### ARnActorModel
A C# Actor Model library.

I was looking for an actor model thing with some requirements :
 - no dependancy from another library
 - coding in plain C#, with such things like 'SendMessage', 'new Actor()', 'Become(behavior)'
 - actor can change their own behaviors
 - actor can send messages
 - actor can create other actors
 - behaviors are dynamic
 - actor can send messages across servers
 
#### With ARnActor, now you can :

    var sender = new BaseActor() ;
    var receiver = new BaseActor() ;
 
 and in sender code ..
 
    receiver.SendMessage("something") ;
 
#### The library holds some useful features :
-  behavior can be attached or removed from actor, (an actor can change it's own behavior ...)
-  actor can send messages across servers, you just need to hold a reference (an IActor interface) to another actor on a server ...
-  some actors can behave as public services, or be supervised
-  ARnActorModel could also be used as a basis for active object, defining an object and casting an async interface around it as a facility. 
-  You need VS2017 to compile ARnActorModel

[![GitHub version](https://badge.fury.io/gh/syndarn%2Farnactormodel.svg)](https://badge.fury.io/gh/syndarn%2Farnactormodel)

[![Build status](https://ci.appveyor.com/api/projects/status/1050h54h8pdfbbh0/branch/master?svg=true)](https://ci.appveyor.com/project/SyndARn/arnactormodel/branch/master)


Unit tests are included as well as some sample applications.
I used the excellent [OpenCover](https://github.com/OpenCover/opencover) to give some tests coverage and it's checked with AppVeyor for continuous integration.
[![Coverage Status](https://coveralls.io/repos/github/SyndARn/ARnActorModel/badge.svg?branch=master)](https://coveralls.io/github/SyndARn/ARnActorModel?branch=master)

For a common usage, you can download ARnActorModel here in Nuget. [![Nuget](https://buildstats.info/nuget/ARnActorModel)](http://nuget.org/packages/ARnActorModel) 


#### Current works

- more coverage
- moving to NetCore/Shared model
- focus on future vs actor
 

