---
uid: Activities
---
# Activities

An [Activity](xref:Marvin.AbstractionLayer.IActivity) is the smallest separately executable step of a Process. Activities are defined as classes derived from `Activity<TParam>` and are always specific to a certain task or application. Activities are instantiated by dedicated modules in their respective domains and then executed by resources. Selection of the resource is usually done using capabilities.

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
