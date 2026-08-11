# Runtime Endpoint

The Runtime Endpoint provides a REST API for managing the MORYX runtime, including modules, databases, and general system information.

## Facades

This endpoint is based on the following facades:

- [`IModuleManager`](/src/Moryx.Runtime/Modules/Management/IModuleManager.cs) - for module management operations
- [`IConfigManager`](/src/Moryx/Configuration/IConfigManager.cs) - for configuration management
- [`IDbContextManager`](/src/Moryx.Model/IDbContextManager.cs) - for database management operations

## Controllers

- **CommonController**: Provides general runtime information (host info, server time)
- **ModulesController**: Manages server modules (start, stop, configure)
- **DatabaseController**: Manages database configuration, creation, and migrations

## Permissions

Permissions are defined in [`RuntimePermissions`](/src/Moryx.Runtime.Endpoints/RuntimePermissions.cs).

The `Moryx.Runtime.Database.CanView` permission controls access to the UI page.

### Common Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Runtime.Common.CanGetGeneralInformation` | Permission for all actions related to getting general runtime information (host info, server time) |

### Module Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Runtime.Modules.CanView` | Permission for all actions related to viewing modules and their state |
| `Moryx.Runtime.Modules.CanViewConfiguration` | Permission for all actions related to viewing module configuration |
| `Moryx.Runtime.Modules.CanViewMethods` | Permission for all actions related to viewing module console methods |
| `Moryx.Runtime.Modules.CanControl` | Permission for all actions related to controlling modules (start, stop, reincarnate) |
| `Moryx.Runtime.Modules.CanConfigure` | Permission for all actions related to configuring modules (save configuration) |
| `Moryx.Runtime.Modules.CanConfirmNotifications` | Permission for all actions related to confirming module notifications |
| `Moryx.Runtime.Modules.CanInvoke` | Permission for all actions related to invoking console methods on modules |

### Database Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Runtime.Database.CanView` | Permission for all actions related to viewing database configuration and state |
| `Moryx.Runtime.Database.CanSetAndTestConfig` | Permission for all actions related to setting and testing database configuration |
| `Moryx.Runtime.Database.CanCreate` | Permission for all actions related to creating databases |
| `Moryx.Runtime.Database.CanErase` | Permission for all actions related to erasing/dropping databases |
| `Moryx.Runtime.Database.CanMigrateModel` | Permission for all actions related to running database migrations |
| `Moryx.Runtime.Database.CanSetup` | Permission for all actions related to executing database setup |
