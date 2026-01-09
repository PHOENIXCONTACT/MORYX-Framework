// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Interface for components within the process engine that are involved in job creation, loading and 
/// </summary>
internal interface IJobHandler
{
    /// <summary>
    /// Work on the list of jobs and modify as needed
    /// </summary>
    /// <param name="jobs">Jobs to handle</param>
    void Handle(LinkedList<IJobData> jobs);
}