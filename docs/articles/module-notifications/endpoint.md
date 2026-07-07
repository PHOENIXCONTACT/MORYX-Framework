# Notifications Endpoint

The Notifications Endpoint provides a REST API for viewing and acknowledging system notifications.

## Facade

This endpoint is based on the [`INotificationPublisher`](/src/Moryx.Notifications/INotificationPublisher.cs) facade for notifications.

## Controllers

- **NotificationPublisherController**: Manages notifications and their acknowledgement

## Permissions

Permissions are defined in [`NotificationPermissions`](/src/Moryx.Notifications.Endpoints/NotificationPermissions.cs).

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Notifications.CanView` | Permission for all actions related to viewing notifications |
| `Moryx.Notifications.CanAcknowledge` | Permission for all actions related to acknowledging notifications |
