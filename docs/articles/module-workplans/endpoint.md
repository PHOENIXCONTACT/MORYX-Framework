# Workplans Endpoint

The Workplans Endpoint provides a REST API for editing and managing workplans graphically.

## Facades

This endpoint is based on the following facades:

- [`IWorkplans`](/src/Moryx/Workplans/API/IWorkplans.cs) - for workplan access
- [`IWorkplanEditing`](/src/Moryx.Workplans/IWorkplanEditing.cs) - for workplan editing operations

## Controllers

- **WorkplanEditingController**: Manages workplan editing operations including steps and connectors

## Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Workplans.CanView` | Permission for all actions related to viewing one or multiple workplans |
| `Moryx.Workplans.CanEdit` | Permission for all actions related to editing or creating one or multiple workplans |
| `Moryx.Workplans.CanDelete` | Permission for all actions related to deleting one or multiple workplans |
