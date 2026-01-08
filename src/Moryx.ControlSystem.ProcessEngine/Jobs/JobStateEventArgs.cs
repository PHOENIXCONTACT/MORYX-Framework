// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Event args for the state change event 
/// </summary>
internal class JobStateEventArgs : EventArgs
{
    /// <summary>
    /// Job that changed its state
    /// </summary>
    public IJobData JobData { get; }

    /// <summary>
    /// Previous state of the job
    /// </summary>
    public IJobState PreviousState { get; }

    /// <summary>
    /// Current state after the change
    /// </summary>
    public IJobState CurrentState { get; }

    /// <summary>
    /// Create JobState event args
    /// </summary>
    public JobStateEventArgs(IJobData job, IJobState previous, IJobState current)
    {
        JobData = job;
        PreviousState = previous;
        CurrentState = current;
    }
}