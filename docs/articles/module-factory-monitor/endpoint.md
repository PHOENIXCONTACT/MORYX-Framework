# Factory Monitor Endpoint

The Factory Monitor Endpoint provides a REST API for monitoring factory operations and status.

## Facades

This endpoint is based on the following facades:

- `IResourceManagement` - for resource data
- `IProcessControl` - for process monitoring
- `IOrderManagement` - for order data

## Controllers

- **FactoryMonitorController**: Provides factory monitoring data and status information

## Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.FactoryMonitor.CanView` | Permission for all actions related to viewing factory monitor data |
