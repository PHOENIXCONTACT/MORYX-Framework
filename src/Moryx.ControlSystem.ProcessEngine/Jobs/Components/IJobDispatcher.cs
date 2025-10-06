// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Public API of the JobDispatcher
    /// </summary>
    internal interface IJobDispatcher : IPlugin
    {
        /// <summary>
        /// Complete initial jobs on reboot
        /// </summary>
        bool RebootCompleteInitial { get; }

        /// <summary>
        /// Load processes
        /// </summary>
        /// <param name="jobData"></param>
        void LoadProcesses(IJobData jobData);

        /// <summary>
        /// Starts a new process for the given job
        /// </summary>
        /// <param name="job">Job to start process</param>
        void StartProcess(IJobData job);

        /// <summary>
        /// Interrupts all processes for the given job
        /// </summary>
        /// <param name="jobData">Job to interrupt processes</param>
        void Interrupt(IJobData jobData);

        /// <summary>
        /// Resumes the given job
        /// </summary>
        void Resume(IJobData jobData);

        /// <summary>
        /// Completes the given job
        /// </summary>
        void Complete(IJobData jobData);

        /// <summary>
        /// Cleans up the given job
        /// </summary>
        void Cleanup(IJobData jobData);

        /// <summary>
        /// Aborts the given job
        /// </summary>
        void Abort(IJobData jobData);
    }
}

