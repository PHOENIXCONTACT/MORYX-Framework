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

