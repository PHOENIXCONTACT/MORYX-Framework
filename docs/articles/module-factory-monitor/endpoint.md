# Factory Monitor Endpoint

The Factory Monitor Endpoint provides a REST API for monitoring factory operations and status.

## Facades

This endpoint is based on the following facades:

- [`IResourceManagement`](/src/Moryx.AbstractionLayer/Resources/IResourceManagement.cs) - for resource data
- [`IProcessControl`](/src/Moryx.ControlSystem/Processes/IProcessControl.cs) - for process monitoring
- [`IOrderManagement`](/src/Moryx.Orders/Facade/IOrderManagement.cs) - for orders and operations

## Controllers

- **FactoryMonitorController**: Provides factory monitoring data and status information

## Permissions

Permissions are defined in [`FactoryMonitorPermissions`](/src/Moryx.FactoryMonitor.Endpoints/FactoryMonitorPermissions.cs).

The `Moryx.FactoryMonitor.CanView` permission controls access to the UI page.

| Permission String | Description |
|-------------------|-------------|
| `Moryx.FactoryMonitor.CanView` | Permission for all actions related to viewing factory monitor data |
