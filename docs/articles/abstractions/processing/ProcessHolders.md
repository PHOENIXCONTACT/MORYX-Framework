---
uid: ProcessHolders
---
# ProcessHolderPosition and ProcessHolderGroup

## Move Processes between Positions

A static class `ProcessMovement` implements transactional movement of processes from one position to another. If an error occurs all involved positions are reset in their previous state.

Example for the Usage is:

````cs
ProcessMovement.Move(sourcePosition, targetPosition);

// Or the explicit transaction API
using (var transaction = ProcessMovement.Transaction(source, target))
{
    // Failed to load mount information
    if (transaction.Progress == MovementProgress.Aborted)
        return false;

    // Optional: Access transaction.MountInformation or start hardware movement

    transaction.MountOnTarget();

    // Optional: Await hardware confirmation

    transaction.RemoveOnSource();

    // Optional: Some other check or milestone

    // Confirm transaction
    transaction.Confirm();

    return transaction.Progress == MovementProgress.Completed;
}
````