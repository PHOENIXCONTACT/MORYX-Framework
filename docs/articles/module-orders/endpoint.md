# Orders Endpoint

The Orders Endpoint provides a REST API for managing orders and operations, including their lifecycle from creation to completion.

## Facade

This endpoint is based on the [`IOrderManagement`](/src/Moryx.Orders/Facade/IOrderManagement.cs) facade for orders and operations.

## Controllers

- **OrderManagementController**: Manages orders, operations, and their state transitions

## Permissions

Permissions are defined in [`OrderPermissions`](/src/Moryx.Orders.Endpoints/OrderPermissions.cs).

The `Moryx.Orders.CanView` permission controls access to the UI page.

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Orders.CanView` | Permission for all actions related to viewing orders and order details |
| `Moryx.Orders.CanViewDocuments` | Permission for all actions related to viewing order documents |
| `Moryx.Orders.CanAdd` | Permission for all actions related to adding new orders |
| `Moryx.Orders.CanManage` | Permission for all actions related to managing orders (update, delete) |
| `Moryx.Orders.CanBegin` | Permission for all actions related to beginning/starting orders and operations |
| `Moryx.Orders.CanInterrupt` | Permission for all actions related to interrupting orders and operations |
| `Moryx.Orders.CanReport` | Permission for all actions related to reporting on orders and operations |
| `Moryx.Orders.CanAdvice` | Permission for all actions related to providing advice on orders and operations |
