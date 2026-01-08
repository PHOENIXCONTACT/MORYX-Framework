// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Central component managing the job behaviors
/// </summary>
internal interface IJobManager : IPlugin
{
    /// <summary>
    /// Configures the job manager with a job scheduler
    /// </summary>
    void Configure(IJobScheduler scheduler);

    /// <summary>
    /// Wait for all jobs to return to their previous state until the given timeout
    /// </summary>
    /// <returns><value>True</value> if the timeout was reached, <value>false</value> if the sync finished before the timeout</returns>
    bool AwaitBoot(int timeoutSec);

    /// <summary>
    /// The job manager can accepts internal jobs before it is started/after it is stopped, but
    /// relies on other components to be already/still running for that. To prevent undefined
    /// behaviour of the module, we provide this flag to be used e.g. by the facade.
    /// </summary>
    bool AcceptingExternalJobs { get; }

    /// <summary>
    /// Adds multiple jobs at once for the jobList queue
    /// </summary>
    Task<IReadOnlyList<IProductionJobData>> Add(JobCreationContext context);
}