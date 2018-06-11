# Constraint Documentation

## Description

[Constraints](xref:Marvin.AbstractionLayer.IConstraint) are used to enrich a [ReadyToWork](xref:Marvin.AbstractionLayer.Resources.ReadyToWork) with
information which are important before dispatching an [Activity](xref:Marvin.AbstractionLayer.IActivity) to a [Resource](xref:Marvin.Resources.IResource).
Since it is not possible to return an already dispatched activity is it necessary to be able to not dispatch that activity.

## ProductConstraint

The [ProductConstraint](xref:Marvin.AbstractionLayer.ProductConstraint) will check the product identity which is given when instantiating 
the ProductConstraint with the one of the [IConstraintContext](xref:Marvin.AbstractionLayer.IConstraintContext).

````cs
public void HandleReadyToWork(IMultiAssembleStation sender, IWpc wpc, int position)
{
    // .....
    IConstraint[] constraints =
    {
        new ProductConstraint
        {
            Identity = product.Identity
        }
    };

    // send the ready to work in name of this resource
    wpc.ReadyToWork(this, position, constraints); // --> next will be HandelStartActivity or HandleFinish
}
````

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