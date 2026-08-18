---
uid: ProductGenericStrategies
---
# Generic Storage Strategies

To speed up development and reduce the need for custom storage strategies, the *ProductManagement* contains generic, reflection based implementations for all strategy types. The strategy can be configured with property strategies mapping to database columns. All properties, that do not have a column configuration are stored in the column defined by JSON column.

## Property Mapper

Property mappers are strategies to map a single property to a database column and back. Per default the *ProductManagement* defines three different mappers. The table below lists those mappers, column type and supported property types.

| Name | Column Type | Supported Property Types |
|---|---|---|
| [IntegerColumnMapper](/src/Moryx.Products.Management/Plugins/GenericStrategies/IntegerColumnMapper.cs) | long | Int16 - UInt64, Enum, DateTime, DateOnly, TimeOnly, bool |
| [FloatColumnMapper](/src/Moryx.Products.Management/Plugins/GenericStrategies/FloatColumnMapper.cs) | double | float, double, decimal |
| [TextColumnMapper](/src/Moryx.Products.Management/Plugins/GenericStrategies/TextColumnMapper.cs) | string | string (plain), Guid, classes/interfaces (JSON), non-primitive structs like Vector3, Quaternion, DateTimeOffset (JSON) |

### IntegerColumnMapper

The `IntegerColumnMapper` stores values as `long` in the database.

- Int16 - UInt64 are stored directly
- Enums are converted to their underlying integer type
- `DateTime` is stored as `Ticks`
- `DateOnly` is stored as `DayNumber` (days since 0001-01-01)
- `TimeOnly` is stored as `Ticks` (ticks since midnight)
- `bool` is stored as `1` or `0`

### FloatColumnMapper

The `FloatColumnMapper` stores values as `double` in the database. Supports `float`, `double` and `decimal` properties.

### TextColumnMapper

The `TextColumnMapper` stores values as `string` in the database.

- `string` is stored directly
- `Guid` is converted to its string representation
- Classes, interfaces and non-primitive structs (e.g. `Vector3`, `Quaternion`, `DateTimeOffset`) are serialized as JSON

## Configuration

The easiest and fastest way to configure the generic strategy is using the `AutoMapper`. This can be achieved through the product management's console on the *MaintenanceWeb* or directly with console commands.

To manually configure a generic strategy for a business object, add an instance of the generic config to the respective strategy collection in the *ProductManagers* configuration. Next select the target type from the drop-down. Per default the JSON column is set to `Text8`, feel free to change this to any other text column. Optionally you can add specific configurations for specific properties using the above mentioned column mappers. Currently there is property name support on the web UI for business object definitions, you will have to correctly configure property names manually.
