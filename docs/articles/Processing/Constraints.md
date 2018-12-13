---
uid: Constraints
---
# Constraints

[Constraints](xref:Marvin.AbstractionLayer.IConstraint) are used to enrich a [ReadyToWork](xref:Marvin.AbstractionLayer.Resources.ReadyToWork) with information which are important before dispatching an [Activity](xref:Marvin.AbstractionLayer.IActivity) to a [Resource](xref:Marvin.Resources.IResource). For that will the [Check()](xref:Marvin.AbstractionLayer.IConstraint.Check) be called which is implemented by several Constraints. The [IConstraintContext](xref:Marvin.AbstractionLayer.IConstraintContext) contains information which will be used to check the constraint. Since it is not possible to return an already dispatched activity is it necessary to be able to not dispatch that activity.

## ExpressionConstraint

The [ExpressionConstraint](xref:Marvin.AbstractionLayer.ExpressionConstraint) can compare a given value with the one returned by a typed lambda expression. It supports standard `Equals` comparison as well was `LessOrEqual` and `GreaterOrEqual`.

````cs
public class DummyContext : IConstraintContext
{
    public int Foo { get; set; }
}

public void ExpressionExamples()
{
    // Define constraints
    var equals = ExpressionConstraint.Equals<DummyContext>(dummy => dummy.Foo, 42);
    var less = ExpressionConstraint.LessOrEqual<DummyContext>(dummy => dummy.Foo, 42);
    var greater = ExpressionConstraint.GreaterOrEqual<DummyContext>(dummy => dummy.Foo, 42);

    // Check constraints
    IConstraintContext context = new DummyContext { Foo = 42 };
    var r1 = equals.Check(context); // True
    var r1 = less.Check(context); // True
    var r1 = greater.Check(context); // True

    context = new DummyContext { Foo = 100 };
    var r2 = equals.Check(context); // False
    var r1 = less.Check(context); // False
    var r1 = greater.Check(context); // True

    context = new DummyContext { Foo = 0 };
    var r2 = equals.Check(context); // False
    var r1 = less.Check(context); // True
    var r1 = greater.Check(context); // False
}
````

## Constraint Context

To define a context for the constraint it is enough to implement the `IConstraintContext` interface. A process implements already the `IConstraintContext` so every process can be used for a check like in the following example:

```` cs
var identity = new ProductIdentity("123456", 0);
var constraint = ExpressionConstraint.Equals<IProcess>(p => ((IProductRecipe) p.Recipe).Product.Identity, identity);

// Prepare a ReadyToWork with the constraint.
var rtw = Session.StartSession(ResourceMode.Production, ReadyToWorkType.Pull, new[]
{
    constraint
});
````

It is also possible to define an application specific constraint for different cases like get activities for a specific order:

```` cs
internal class OrderConstraint : IConstraint
{
    private readonly string _orderNumber;

    public OrderConstraint(string orderNumber)
    {
        _orderNumber = orderNumber;
    }

    public bool Check(IConstraintContext context)
    {
        var setupRecipe = (context as IProcess)?.Recipe as ISetupRecipe;
        if (setupRecipe == null)
            return false;

        return ((IOrderBasedRecipe)setupRecipe.TargetRecipe).OrderNumber.Equals(_orderNumber);
    }
}
````

And then prepare a ReadyToWork with the application specific constraint.

```` cs
var rtw = Session.StartSession(ResourceMode.Production, ReadyToWorkType.Pull, new[]
{
    new OrderConstraint("1234567891234")
});
````