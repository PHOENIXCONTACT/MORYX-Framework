// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders;

/// <summary>
/// Represents the progress of an operation
/// </summary>
public class OperationProgress
{
    /// <summary>
    /// Sum of RunningCount of jobs.
    /// </summary>
    public virtual int RunningCount { get; protected set; }

    /// <summary>
    /// Sum of SuccessCounts of jobs.
    /// </summary>
    public virtual int SuccessCount { get; protected set; }

    /// <summary>
    /// Sum of FailureCounts of jobs
    /// </summary>
    public virtual int FailureCount { get; protected set; }

    /// <summary>
    /// Sum of ReworkedCount of jobs
    /// </summary>
    public virtual int ReworkedCount { get; protected set; }

    /// <summary>
    /// Scrap parts of this operation
    /// </summary>
    public virtual int ScrapCount { get; protected set; }

    /// <summary>
    /// Pending parts for this operation
    /// </summary>
    public virtual int PendingCount { get; protected set; }

    /// <summary>
    /// Running parts of all jobs
    /// </summary>
    public virtual int ProgressRunning { get; protected set; }

    /// <summary>
    /// Success parts of all jobs
    /// </summary>
    public virtual int ProgressSuccess { get; protected set; }

    /// <summary>
    /// Scrap parts of all jobs
    /// </summary>
    public virtual int ProgressScrap { get; protected set; }

    /// <summary>
    /// Pending parts of all jobs
    /// </summary>
    public virtual int ProgressPending { get; protected set; }
}
