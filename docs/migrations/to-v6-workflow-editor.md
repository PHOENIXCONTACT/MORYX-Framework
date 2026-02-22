# Migrate to WorkplanEditor 6.x.x

## `IWorkplanEditing` facade

### Removed `position`

The `IWorkplanEditing` facade now takes into account, that the `IWorkplanNode`
(and thus `IWorkplanStep`) has a `Position` property. Considering this, all
calls that took a workplan step and related position have been changed by
removing the `Point position` parameter.

Instead of having the position separated, make sure you assign position 
coordinates directly to your `IWorkplanNode` implementing and derived types.

#### Database changes

Because the `AbstractionLayer` (`Moryx.Products.Model`) now takes
responsibility of persisting the positions of workplan steps and connectors,
consider to migrate your current position related data.

**Hint:** Since these changes should only affect the UI and are not relevant
for operation, you might stick with the default values (initial value is `0`)
and rearrange step positions manually or when needed.
