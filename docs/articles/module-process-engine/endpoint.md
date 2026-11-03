# Process Engine Endpoint

The Process Engine Endpoint provides a REST API for managing jobs and processes within the control system.

## Facades

This endpoint is based on the following facades:

- [`IJobManagement`](/src/Moryx.ControlSystem/Jobs/IJobManagement.cs) - for job management operations
- [`IProcessControl`](/src/Moryx.ControlSystem/Processes/IProcessControl.cs) - for process control and monitoring

## Controllers

- **JobManagementController**: Manages production and setup jobs
- **ProcessEngineController**: Provides access to process information and history

## Permissions

Permissions are defined in [`JobPermissions`](/src/Moryx.ControlSystem.Jobs.Endpoints/JobPermissions.cs) and [`ProcessPermissions`](/src/Moryx.ControlSystem.Processes.Endpoints/ProcessPermissions.cs).

The `Moryx.Processes.CanView` permission controls access to the UI page.

### Job Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Jobs.CanView` | Permission for all actions related to viewing jobs and job details |
| `Moryx.Jobs.CanComplete` | Permission for all actions related to completing jobs |
| `Moryx.Jobs.CanAbort` | Permission for all actions related to aborting jobs |

### Process Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Processes.CanView` | Permission for all actions related to viewing processes and process information |

## Process Holders

The `Moryx.ControlSystem.Processes.Endpoints` package provides a REST API to interact with `ProcessHolderPosition` and `ProcessHolderGroup` resources.

### Setup

Add the `Moryx.ControlSystem.Processes.Endpoints` package reference to your start project.

### Features

- Get all process holder positions with their corresponding group or parent resource
- Subscribe to `ProcessHolderPosition` process update events
- Reset a `ProcessHolderPosition`
- Reset a `ProcessHolderGroup`, which automatically resets all its `ProcessHolderPosition`s

### Custom Icons

You can place the `EntryVisualizationAttribute` on your custom `ProcessHolderGroup` implementation or on the `Resource` that holds the `ProcessHolderPosition` to display a custom icon in the UI. Icon names should come from [Material Symbols](https://fonts.google.com/icons).

```cs
[ResourceRegistration]
[EntryVisualization(null, "user_icon")]
public class MyResource : Cell
{
    [DataMember]
    public IProcessHolderPosition SlotA { get; set; }

    [DataMember]
    public IProcessHolderPosition SlotB { get; set; }
}
```
