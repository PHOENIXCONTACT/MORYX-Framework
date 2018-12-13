---
uid: Tasks
---
# Tasks

[Tasks](xref:Marvin.AbstractionLayer.ITask) are the elements to define a workplan which defines to production flow of a product. Each task has its corresponding activity which will be created from a Task. So the Workplan will be processed step by step and every task will create the corresponding activity which will be handled by a resource.

## Create a Task

A task is like the activity an application specific class. It is derived from the `TaskStep<TActivity, TParam>` class and must be created to get the needed activities like in the following example:

```` cs
public class MyTask : TaskStep<MyActivity, MyParameters>
{
    public override string Name => nameof(MyTask);
}
````

If the activity is derived from `AssembleActivity` and has no parameters then it is enough to use the `AssembleParameters` class like:

```` cs
public class MyTask : TaskStep<MyActivity, AssembleParameters>
{
    public override string Name => nameof(MyTask);
}
````

After creating the class it will be available at the production flow editor to create the needed workplan.