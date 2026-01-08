// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Central component to access the job database
    /// </summary>
    internal interface IJobStorage : IPlugin
    {
        /// <summary>
        /// Get all active, non-completed jobs
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IJobData> GetAll();

        /// <summary>
        /// Save jobs to the database and link them properly
        /// </summary>
        void Save(ModifiedJobsFragment modifiedJobs);

        /// <summary>
        /// Only update a jobs state in the database
        /// </summary>
        void UpdateState(IJobData jobData, IJobState jobState);
    }
}
