---
uid: VisualInstructions
---
# Visual Instructions

## Binding

Visual instructions can use binding to include values from the process into text instructions. Each binding must have the form `"{Base.Property}"` where
`Base` can have four different values that define which object is used to resolve the binding. The property is optional and if present must refer to
a property with a public getter on the object.

| Base-Key | Target |
|----------|--------|
| Process  | The process the parameters are bound on. |
| Recipe   | The recipe of the bound process. |
| Product  | The product type if it is a ProductionProcess. |
| Article  | The article instance in case of a ProductionProcess. |

## Instruction Examples

Below are a couple of examples for instructions using bindings.

````cs
var sample = new VisualInstruction
{
    Type = InstructionContentType.Text,
    Content = "Please remove article with serial number {Article.Identity} from the carrier."
}
sample = new VisualInstruction
{
    Type = InstructionContentType.Text,
    Content = "Please insert material of type {Product.Identity} for order {Recipe.OrderNumber} on the carrier."
}
sample = new VisualInstruction
{
    Type = InstructionContentType.Text,
    Content = "Article for process {Process.Id} is in state {Article.State}!"
}
````

## Binding Resolution

When a new activity type is defined, the type of the activity parameters must be provided through the generic `Activity` base class:

````cs
[ActivityResults(typeof(DefaultActivityResult))]
public class SolderingActivity : Activity<SolderingActivityParameters>
{
}
````

If the activity is an activity with visual instructions support, the activity parameters type must derive from [VisualInstructionParameters](/src/Moryx.VisualInstructions/VisualInstructionParameters.cs):

````cs
public class SolderingActivityParameters : VisualInstructionParameters
{
}
````

This base class provides the binding resolution for the binding sources in the table above.



