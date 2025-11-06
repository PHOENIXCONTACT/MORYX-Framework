# MORYX 8.x to 10.x

## Replaced result of visual instructions with dedicated result object

In *Moryx.Factory* **6.3** and **8.1** we introduced the new result object and optional extended APIs. The result object solved issues caused by localization of the different results. With **Moryx 10** we remove all old APIs based on strings.

## Replaced `IVisualInstructions` with `VisualInstructionParameters`
The interface was only used in `VisualInstructionParameters` which can and is being used as a base class in most cases anyway.
Hence, `IVisualInstructions` is removed in favor of a more extendable base class.

## Integration of Moryx.Simulation into Moryx.ControlSystem

To reduce the number of API packages and simplify the overall architecture, **Moryx.Simulation** has been integrated into **Moryx.ControlSystem** starting with Moryx 10. All simulation-related APIs and functionality are now part of the Moryx.ControlSystem package. This change streamlines dependency management and makes it easier to maintain and extend simulation features within the control system context.

The simulator module has also been renamed, and its namespace and package id have changed accordingly to reflect its new location within Moryx.ControlSystem.


## Renamings and Typo-Fixes

- TcpClientConfig.IpAdress -> TcpClientConfig.IpAddress
- TcpListenerConfig.IpAdress -> TcpListenerConfig.IpAddress
- ResourceRelationType.CurrentExchangablePart -> ResourceRelationType.CurrentExchangeablePart
- ResourceRelationType.PossibleExchangablePart -> ResourceRelationType.PossibleExchangeablePart
- MqttDriver.BrokerURL -> MqttDriver.BrokerUrl

## Reduction of interfaces

Several interfaces have been removed to streamline the codebase and reduce complexity. The following interfaces are no longer available:

- `IProductType`: Replaced with base-class `ProductType`
- `IProductInstance`: Replaced with base-class `ProductInstance`

## Data Model Changes

With MORYX 10, several changes have been made to the data model to improve performance and maintainability. Notable changes include:

- Naming conventions across the data model have been standardized to ensure consistency and clarity. We use pluralised names for DbSet properties and singular names for entity classes. Sample: `public DbSet<ProductEntity> Products { get; set; }` instead of `public DbSet<ProductEntity> ProductEntities { get; set; }`.
- Derived data models are now fully supported. This needs some changes how the model is defined in code first scenarios. Please refer to the [Data Model Tutorial](/docs/tutorials/data-model/CodeFirst.md) for more information.

### Removal of ProductFileEntity

The `ProductFileEntity` has been removed from the data model as it was not utilized effectively. This change helps streamline the data structure and reduce unnecessary complexity.
