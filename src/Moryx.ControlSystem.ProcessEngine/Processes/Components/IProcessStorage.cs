// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Workplans;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.Model.Repositories;
using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Storage to persist activities and processes to the database
    /// </summary>
    internal interface IProcessStorage : IPlugin
    {
        /// <summary>
        /// Get all active processes of a job
        /// </summary>
        IReadOnlyList<ProcessData> GetRunningProcesses(IUnitOfWork uow, IJobData job);

        /// <summary>
        /// Load completed processes of a job from the database into the collection
        /// </summary>
        void LoadCompletedProcesses(IUnitOfWork uow, IJobData jobData, ICollection<ProcessData> allProcesses);

        /// <summary>
        /// Load all completed activities of the process
        /// </summary>
        IReadOnlyList<ActivityData> LoadCompletedActivities(IUnitOfWork uow, ProcessData processData, IDictionary<long, ITask> taskMap);

        /// <summary>
        /// Reload a configured activity instead of creating a new entity. If we did not save an activity for the
        /// task yet, this method creates a new entity instead
        /// </summary>
        void TryReloadRunningActivity(long processId, ActivityData activityData);

        /// <summary>
        /// Load all completed activities of the process
        /// </summary>
        void FillActivities(IUnitOfWork uow, Process process, IDictionary<long, ITask> taskMap);

        /// <summary>
        /// Load tracing object from database
        /// </summary>
        Tracing LoadTracing(ActivityEntity activityEntity);

        /// <summary>
        /// Update process in database
        /// </summary>
        /// <param name="processData">Process to save</param>
        void SaveProcess(ProcessData processData);

        /// <summary>
        /// Append information about a completed activity to the process
        /// </summary>
        void AddCompletedActivity(ProcessData processData, ActivityData activityData);
    }
}
