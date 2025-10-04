# ResourceReference

`ResourceReference` is an attribute that links 2 or more resources together. 
it's a mechanism that is used to save and load the source and the target relationship in the database.

## How does it work?
Let's take a look at this example:
```cs
    [ResourceRegistration] 
    public class AssemblingCell : Cell
    {
        [ResourceReference(ResourceRelationType.Extension)]
        public IVisualInstructor Instructor { get; set; }
        ....
    }
```

The `AssemblyCell` has a property of type `IVisualInstructor` and a `ResourceReference` attribute 
of  `ResourceRelationType` `Extension`. 
Behind the scene the resource manager saves only the `ResourceRelationType`, 
the `AssemblyCell` id, and the `Instructor` id in the database. 
And uses the same information to load a resource and it's reference from the database.
In this case at load time, the resource manager checks for all the resources 
that implement the `IVisualInstructor` interface, then filters them by `ResourceRelationType` 
which in this case is `Extension`.

![Resource Reference in the Database](./images/resource%20reference.PNG)