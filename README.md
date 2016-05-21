#### ARnActorModel
A C# Actor Model library.

I was looking for an actor model things with some requirements :
 - no dependancy from another library
 - coding in plain C#, with such things like 'SendMessage', 'new Actor()', 'Become(behavior)'
 - actor can change behaviors
 - actor can send message
 - actor can create other actors
 - behaviors are dynamic
 - actor can send message across servers
 
#### With ARnActor, now you can :

    var sender = new BaseActor() ;
    var receiver = new BaseActor() ;
 
 and in sender code ..
 
    receiver.SendMessage("something") ;
 
#### The library holds some useful features :
-  behavior can be attached or removed from actor, (an actor can change it's own behavior ...)
-  actor can send messages across servers, you just need to hold a reference to another actor on a server ...
-  some actor can behave as public services, or be supervised

[![GitHub version](https://badge.fury.io/gh/syndarn%2Farnactormodel.svg)](https://badge.fury.io/gh/syndarn%2Farnactormodel)

[![Build status](https://ci.appveyor.com/api/projects/status/1050h54h8pdfbbh0/branch/master?svg=true)](https://ci.appveyor.com/project/SyndARn/arnactormodel/branch/master)


Unit tests are included as well as some sample applications.
I used the excellent [OpenCover](https://github.com/OpenCover/opencover) to give some tests coverage.
[![Coverage Status](https://coveralls.io/repos/github/SyndARn/ARnActorModel/badge.svg?branch=master)](https://coveralls.io/github/SyndARn/ARnActorModel?branch=master)

For a common usage, you can download ARnActorModel here in Nuget. [![Nuget](https://buildstats.info/nuget/ARnActorModel)](http://nuget.org/packages/ARnActorModel) 


#### Current works

- more coverage
- moving to PCL/Universal/Shared model
 

