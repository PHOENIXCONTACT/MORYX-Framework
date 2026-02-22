---
uid: Constraints
---
# Constraints

[Constraints](/src/Moryx.AbstractionLayer/Constraints/IConstraint.cs) as well as [Capabilities](/src/Moryx.AbstractionLayer/Capabilities/ICapabilities.cs) are used to filter the possible matches of [Resources](/src/Moryx.AbstractionLayer/Resources/IResource.cs) and [Activities](/src/Moryx.AbstractionLayer/Activities/IActivity.cs).
In Contrast to capabilities which are sets of provided and required abilities, constraints are boolean conditions which arise from the system's context.
To check if the context's constraints are met, the [Check()](/src/Moryx.AbstractionLayer/Constraints/IConstraint.cs) method of the constraint is called.
The [IConstraintContext](/src/Moryx.AbstractionLayer/Constraints/IConstraintContext.cs) contains all the information which is needed to perform this check.

## ExpressionConstraint

The [ExpressionConstraint](/src/Moryx.AbstractionLayer/Constraints/ExpressionConstraint.cs) provides an example for the usage of constraints.
It can compare a given value with the one returned by a typed lambda expression and supports standard `Equals` comparison as well was `LessOrEqual` and `GreaterOrEqual`.
The `DummyContext` provides the value to check against. 

````cs
public class DummyContext : IConstraintContext
{
    public int Foo { get; set; }
}

public void ExpressionExamples()
{
    // Define constraints
    var equals = ExpressionConstraint.Equals<DummyContext>(dummy => dummy.Foo, 42);
    var lessOrEqual = ExpressionConstraint.LessOrEqual<DummyContext>(dummy => dummy.Foo, 42);
    var greaterOrEqual = ExpressionConstraint.GreaterOrEqual<DummyContext>(dummy => dummy.Foo, 42);

    // Check constraints
    IConstraintContext context = new DummyContext { Foo = 42 };
    var r1 = equals.Check(context); // True
    var r1 = lessOrEqual.Check(context); // True
    var r1 = greaterOrEqual.Check(context); // True

    context = new DummyContext { Foo = 100 };
    var r2 = equals.Check(context); // False
    var r2 = lessOrEqual.Check(context); // False
    var r2 = greaterOrEqual.Check(context); // True

    context = new DummyContext { Foo = 0 };
    var r3 = equals.Check(context); // False
    var r3 = lessOrEqual.Check(context); // True
    var r3 = greaterOrEqual.Check(context); // False
}
````

## Constraint Context

To define a context for the constraint it is enough to implement the [IConstraintContext](/src/Moryx.AbstractionLayer/Constraints/IConstraintContext.cs) interface. 
Since [Processes](/src/Moryx.AbstractionLayer/Process//IProcess.cs) implement the [IConstraintContext](/src/Moryx.AbstractionLayer/Constraints/IConstraintContext.cs) interface, every process can be used for a check like in the following example:

```` cs
var identity = new ProductIdentity("123456", 0);
var constraint = ExpressionConstraint.Equals<IProcess>(p => ((IProductRecipe) p.Recipe).Product.Identity, identity);
````