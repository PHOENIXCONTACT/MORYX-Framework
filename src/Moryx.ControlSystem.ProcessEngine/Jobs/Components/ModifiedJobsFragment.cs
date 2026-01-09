// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

internal class ModifiedJobsFragment
{
    /// <summary>
    /// Reference id for the first job in <see cref="AffectedJobs"/>
    /// </summary>
    public long? PreviousId { get; }

    /// <summary>
    /// All jobs that were added, modified or moved
    /// </summary>
    public IReadOnlyList<IJobData> AffectedJobs { get; }

    /// <summary>
    /// Save jobs somewhere within the job list
    /// </summary>
    public ModifiedJobsFragment(IReadOnlyList<IJobData> affectedJobs, long? previousId)
    {
        AffectedJobs = affectedJobs;
        PreviousId = previousId;
    }
}