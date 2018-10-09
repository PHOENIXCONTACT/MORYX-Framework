# AbstractionLayer

## Description

The AbstractionLayer package itself inside the AbstractionLayer solution is a collection 
of interfaces, base classes and common implementations of objects needed by the AbstractionLayer solution, 
the MaRVIN ControlSystem and other derived solutions.

## Activities

An [Activity](xref:Marvin.AbstractionLayer.IActivity) is the smallest separately executable step 
of a Process. Activities are defined as classes derived from `Activity<TParam` and are always specific 
to a certain task or application. Activities are instantiated by dedicated modules in their respective
domains and then executed by resources. Selection of the resource is usually done using capabilities.

For long-term tracibility and to resume interrupted activities it is possible to use `IActivityTracing`.
Using the 32bit integer `Progress` of the base class `Tracing` derived types can trace intermediate progress
during activity execution. The example below shows how to define an enum for the progress.

````cs
public enum FooProgress
{
    Initial = 0,
    Running = 50,
    Done = 100
}
public class FooTracing : Tracing, IActivityProgress
{
    public new FooProgress Progress
    {
        get { return (FooProgress)base.Progress; }
        set { base.Progress = (int)value; }
    }

    // Relative progress defined by IActivityProgress
    public double Relative => base.Progress;
}
````

Resources can access and transform an activities tracing information using the fluent API `Transform`
and `Trace`. This rather complex approach is taken because the type of the tracing object might change
at runtime depending on the resource executing the activity or the circumstances of the execution.
For the example above this would look like this:

````cs
FooTracing tracing = activity.TransformTracing<FooTracing>()
  .Trace(t => t.Progress = FooProgress.Running);
````

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

[Identities](xref:Marvin.AbstractionLayer.Identity.IIdentity) are used to represent unique properties like serial numbers and MAC adresses for Articles and material number for Products. There is also the derived type [ProductIdentity](xref:Marvin.AbstractionLayer.ProductIdentity) that represents a products material number and revision. The static constructor `AsLatestRevision` also lets you refer to the latest revision for a certain material.

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
