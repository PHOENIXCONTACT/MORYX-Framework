---
uid: ProductGenericStrategies
---
# Generic Storage Strategies

To speed up development and reduce the need for custom storage strategies, the *ProductManagement* contains generic, reflection based implementations for all strategy types. The strategy can be configured with property strategies mapping to database columns. All properties, that do not have a column configuration are stored in the column defined by JSON column.

## Property Mapper

Property mappers are strategies to map a single property to a database column and back. Per default the *ProductManagement* defines three different mappers. The table below lists those mappers, column type and supported property types.

| Name | Column Type | Supported Property Types |
|---|---|---|
| [IntegerColumnMapper](../../../src/Moryx.Products.Management/Plugins/GenericStrategies/IntegerColumnMapper.cs) | long | Int16 - UInt64 , Enum, DateTime, bool |
| [FloatColumnMapper](../../../src/Moryx.Products.Management/Plugins/GenericStrategies/FloatColumnMapper.cs) | double | Float, Double, Decimal |
| [TextColumnMapper](../../../src/Moryx.Products.Management/Plugins/GenericStrategies/TextColumnMapper.cs) | string | string (plain), object (JSON) |

## Configuration

The easiest and fastest way to configure the generic strategy is using the `AutoMapper`. This can be achieved through the product managements console on the *MaintenanceWeb* or directly with console commands.

To manually configure a generic strategy for a business object, add an instance of the generic config to the respective strategy collection in the *ProductManagers* configuration. Next select the target type from the drop-down. Per default the JSON column is set to `Text8`, feel free to change this to any other text column. Optionally you can add specific configurations for specific properties using the above mentioned column mappers. Currently there is property name support on the web UI for business object definitions, you will have to correctly configure property names manually.