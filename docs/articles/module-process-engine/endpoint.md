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
