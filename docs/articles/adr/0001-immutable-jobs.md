# ADR: Immutable Jobs

In the design of the ProcessEngine there was the reoccurring question whether or not jobs can be altered after creation. 

## Status

**Accepted**

## Decision

We declare jobs immutable. After creation its attributes like recipe and amount **must not** be altered. It is also not possible to pause them once started and resume them later. Jobs are automatically considered startable after creation, external approval or interference is neither necessary nor possible.

## Consequences

Limiting external interference reduces complexity of the ProcessEngines internal implementation and increases stability. By considering jobs directly startable after creation without any external influences the state machine and job scheduler could be simplified. On the downside the job list can not be used to create jobs for later execution. The inability to pause jobs also means that external users of the ProcessEngine have to interrupt running jobs and create new ones with the remaining count if they want to pause execution.