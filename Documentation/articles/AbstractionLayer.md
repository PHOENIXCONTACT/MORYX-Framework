# AbstractionLayer

## Description

The AbstractionLayer package itself inside the AbstractionLayer solution is a collection 
of interfaces, base classes and common implementations of objects needed by the AbstractionLayer solution, 
the MaRVIN ControlSystem and other derived solutions.

## Activities

An [Activity](xref:Marvin.AbstractionLayer.IActivity) is the smallest separately executable step 
of a Process.
An activity is executed by a resource. To find a resource capable to execute an activity, both define 
a set of needed resp. provided Capabilities. The ControlSystem's 
ProcessController tries to find a matching resource for each activity. If there is more than one, an 
optimizer strategy may be used to selected the "best" matching one.

## Articles

An [Article](xref:Marvin.AbstractionLayer.Article) is the produced instance of a Product.

## Capabilities

[Capabilities](xref:Marvin.AbstractionLayer.ICapabilities) are used to find a matching *Resource* for an Activity.

## Conditions

[Conditions](xref:Marvin.AbstractionLayer.ICondition) may be attached to an Activity. An Activity may be executed if and only if all attached conditions are fullfilled, which means that the
[Check()](xref:Marvin.AbstractionLayer.ICondition) method of all
conditions must return true.

## Constraints

[Constraints](xref:Marvin.AbstractionLayer.IConstraint) may be used when calling a
ReadyToWork to be able to restrict the possible activities which could be dispatched
to the resource which has called the ReadyToWork. For that will the
[Check()](xref:Marvin.AbstractionLayer.IConstraint.Check)
be called which is implemented by several Constraints. The [IConstraintContext](xref:Marvin.AbstractionLayer.IConstraintContext)
contains information which will be used to check the constraint.

## Identity

[Identities](xref:Marvin.AbstractionLayer.Identity.IIdentity) are used to represent unique properties like serial numbers and MAC adresses for Articles and material number for Products.

## Process

A [Process](xref:Marvin.AbstractionLayer.IProcess) consists of a series of Activities and is managed by the MaRVIN ControlSystem's ProcessController.

## Products

A [Product](xref:Marvin.AbstractionLayer.IProduct) or better a *product description* is used by a [ProductRecipes](xref:Marvin.AbstractionLayer.ProductRecipe) together with a Workplan to define how to produce an Article.

## Recipes

A [Recipe](xref:Marvin.AbstractionLayer.Recipe) is used to combine a Workplan and a set of parameters to define all the [Activities](xref:abstractionlayer-Activities) needed for a Process

## Resources

The Resources package of the AbstrationLayer contains several basic classes to be used by the [Resources](xref:Marvin.Resources.IResource) and the [ResourceManager](xref:Marvin.Resources.IResourceManagement)

## Tasks

[Tasks](xref:Marvin.AbstractionLayer.ITask) are used within a Workplan to define the Activity for each step.

## Workplan

The Workplan package of the AbstrationLayer contains several helper classes to be used by the Workflow package.
