---
uid: ResourceTypesOverview
---
# Overview

This article describes different branches of the [Resource type tree](../ResourceTypeTree.md). 
Those branches are types derived from either [Resource](xref:Moryx.AbstractionLayer.Resources.Resource) or [PublicResource](xref:Moryx.AbstractionLayer.Resources.PublicResource). 
Each of those branches contains a certain type of resources that share common traits or represent similar objects. 
The `AbstractionLayer` deﬁnes some resource types that create branches of resources and applications from different domains can deﬁne additional branches for resources from their respective business ﬁelds. 
In a manufacturing environment an abstract resource type `Cell` as a branch in the type tree could be considered to model different stations in a production line.

Go deeper into resource types with the following articles which present branches that are included in the `AbstractionLayer`:

* [Driver resource](DriverResource.md) - Communicate with another device
* [Interaction resource](InteractionResource.md) - A resource to interact with an UI

Have also a look on the tutorial [how to create a resource](../../Tutorials/HowToCreateResource.md).