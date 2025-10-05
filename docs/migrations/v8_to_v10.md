# MORYX 8.x to 10.x

## Replaced result of visual instructions with dedicated result object

In *Moryx.Factory* **6.3** and **8.1** we introduced the new result object and optional extended APIs. The result object solved issues caused by localization of the different results. With **Moryx 10** we remove all old APIs based on strings.

## Replaced `IVisualInstructions` with `VisualInstructionParameters`
The interface was only used in `VisualInstructionParameters` which can and is being used as a base class in most cases anyway.
Hence, `IVisualInstructions` is removed in favor of a more extendable base class.

## Integration of Moryx.Simulation into Moryx.ControlSystem

To reduce the number of API packages and simplify the overall architecture, **Moryx.Simulation** has been integrated into **Moryx.ControlSystem** starting with Moryx 10. All simulation-related APIs and functionality are now part of the Moryx.ControlSystem package. This change streamlines dependency management and makes it easier to maintain and extend simulation features within the control system context.

The simulator module has also been renamed, and its namespace and package id have changed accordingly to reflect its new location within Moryx.ControlSystem.

## Removal of WorkerSupport Module

The **WorkerSupport** module has been removed in Moryx 10. Endpoints that previously relied on WorkerSupport to access instructors now retrieve instructors directly from the resource management. This change simplifies the architecture and reduces indirection, making instructor access more straightforward and maintainable.

