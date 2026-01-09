// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

/// <summary>
/// Get jobs from the history like completed jobs
/// </summary>
internal interface IJobHistory
{
    /// <summary>
    /// Returns a requested job from the history
    /// </summary>
    /// <returns>The requested job or <c>null</c> if there is no job with the given ID.</returns>
    Job Get(long jobId);
}