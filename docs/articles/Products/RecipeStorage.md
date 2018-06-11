# Recipe Storage

## Workplans

### Model structure

Within the product model workplans are represented by six tables. `WorkplanEntity` represents workplans and `WorkplanReference` contains information about new versions and copies of workplans. The `Workplan` itself is, similiar to the business object, represented by `StepEntity`, `ConnectorEntity` and `ConnectorReference`. While `StepEntity` is mapped directly to `WorkplanStep` and `ConnectorEntity` to `IConnector`, the `ConnectorReference` only exists in the database. It stores reference information for the `IWorkplanStep.Inputs` and `IWorkplanStep.Outputs` to their connectors. `StepEntity` additionaly has a collection of `OutputDescriptions` which are also mapped directly to the property.

### Saving Workplans

When a workplan is saved to the database the base entity is created and filled with properties like `Name` and `State`. Next the `Connectors` and `Steps` are transformed to entities and written to maps of `NodeId` => `Entity`. In a final step all references between `Steps` and `Connectors` are written to the `ConnectorReference` table.

When saving a step its type is determined and saved in the three columns `Assembly`, `NameSpace` and `Classname`. This allows for easy update scripts in case of refactorings.

### Loading Workplans

Loading a workplan follows a similar approach like saving it. First all `Steps` and `Connectors` are transformed back and then the object graph is restored from the `ConnectorReference` table.

Finally the `Workplan` object is restored and properties like `Name` and `State` written back.