// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.ProcessEngine.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// The main functionality of the ProcessController module.
    /// </summary>
    internal interface IProcessController
    {
        /// <summary>
        /// Load all processes of this job
        /// </summary>
        IReadOnlyList<ProcessData> LoadProcesses(IJobData job);

        /// <summary>
        /// Starts a created process AFTER it was referenced in the job
        /// </summary>
        void Start(ProcessData process);

        /// <summary>
        /// Remove leftover processes for the job after an uncontrolled shutdown, and create UnmountActivities if necessary.
        /// </summary>
        void Cleanup(IEnumerable<ProcessData> processes);

        /// <summary>
        /// Stops execution of the process. The user can decide whether to remove the piece from
        /// the machine or leave it there.
        /// </summary>
        void Interrupt(IEnumerable<ProcessData> processes, bool abort);

        /// <summary>
        /// Resumes execution of loaded job and returns all running processes
        /// </summary>
        void Resume(IEnumerable<ProcessData> processes);

        /// <summary>
        /// Occurs when the process state changed
        /// </summary>
        event EventHandler<ProcessEventArgs> ProcessChanged;
    }
}
