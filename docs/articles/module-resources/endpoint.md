# Resources Endpoint

The Resources Endpoint provides a REST API for managing the resource graph, including viewing, creating, editing, and deleting resources.

## Facade

This endpoint is based on the [`IResourceManagement`](/src/Moryx.AbstractionLayer/Resources/IResourceManagement.cs) facade for resource management.

## Controllers

- **ResourceModificationController**: Manages resource instances, the resource tree, and resource methods

## Permissions

Permissions are defined in [`ResourcePermissions`](/src/Moryx.AbstractionLayer.Resources.Endpoints/ResourcePermissions.cs).

The `Moryx.Resources.CanViewTree` permission controls access to the UI page.

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Resources.CanViewTree` | Permission for all actions related to viewing the resource instance tree |
| `Moryx.Resources.CanViewDetails` | Permission for all actions related to viewing the instance information of a resource |
| `Moryx.Resources.CanAddResource` | Permission for all actions related to viewing the resource type tree |
| `Moryx.Resources.CanAdd` | Permission for all actions related to adding one or multiple resources |
| `Moryx.Resources.CanEdit` | Permission for all actions related to editing the resource graph and its members |
| `Moryx.Resources.CanDelete` | Permission for all actions related to adding one or multiple resources |
| `Moryx.Resources.CanInvokeMethod` | Permission for all actions related to invoking a method on a resource |
