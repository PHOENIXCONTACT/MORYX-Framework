---
uid: Constraints
---
# Constraints

[Constraints](xref:Marvin.AbstractionLayer.IConstraint) may be used when calling a
ReadyToWork to be able to restrict the possible activities which could be dispatched
to the resource which has called the ReadyToWork. For that will the
[Check()](xref:Marvin.AbstractionLayer.IConstraint.Check)
be called which is implemented by several Constraints. The [IConstraintContext](xref:Marvin.AbstractionLayer.IConstraintContext)
contains information which will be used to check the constraint.