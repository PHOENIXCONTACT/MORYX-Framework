---
uid: ResourceTypesOverview
---
# Overview

This article describes different branches of the [Resource type tree](../ResourceTypeTree.md). 
Those branches are types derived from [Resource](/src/Moryx.AbstractionLayer/Resources/Resource.cs).
Each of those branches contains a certain type of resources that share common traits or represent similar objects. 
The `AbstractionLayer` defines some resource types that create branches of resources and applications from different domains can define additional branches for resources from their respective business fields. 
In a manufacturing environment an abstract resource type [Cell](/src/Moryx.ControlSystem/Cells/Cell.cs) as a branch in the type tree could be considered to model different stations in a production line.
Similarly the [Driver resource](DriverResource.md) creates a branch for resources that represent hardware devices with their communication.

Have also a look on the tutorial [how to create a resource](/docs/tutorials/HowToCreateResource.md).