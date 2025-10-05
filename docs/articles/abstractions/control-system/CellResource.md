# Cell Resource

A cell is a single unit within a manufacturing system that executes activities of a process. The contract between the resource implementation and the process engine is described by the `ICell` interface. It is recommended to derive from `Cell` instead of implementing the interface directly.

Cells need to export the provided capabilities, either during `OnInitialize` or once all necessary conditions are met (driver connected, setup completed, ...).

## Cell Session Sequence

The interaction between a cell and the process engine to execute an activity is represented by the session class and its derived type. 

1. Each sequence begins with a `ReadyToWork` published by the cell. In case of `ReadyToWorkType.Pull` the process engine responds with either `ActivityStart` or `SequenceCompleted`, depending on whether or not there is a match for the started session.
   1. For `ReadyToWorkType.Push` the process engine stores the session until there is a match.
   2. A push event can be revoked via `NotReadyToWork`
2. The cell executes the activity wrapped by `ActivityStart` and reports its result upon completion by raising `ActivityCompleted`.
3. The control system closes the sequence with `SequenceCompleted`. The object also indicates if the process is still active (`ProcessActive`) and where it should go next (`NextCells`).
4. The session can be continued, but that only makes sense if the process is still active and the current cell one of the next cells.