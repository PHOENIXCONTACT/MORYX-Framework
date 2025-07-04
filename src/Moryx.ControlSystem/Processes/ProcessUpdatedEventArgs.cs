// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Enum representing the progress of a process within the kernel
    /// </summary>
    public enum ProcessProgress
    {
        /// <summary>
        /// Process has passed all preparation stages and is ready for production
        /// </summary>
        Ready,
        /// <summary>
        /// Process is running and activities have been dispatched
        /// to cells
        /// </summary>
        Running,
        /// <summary>
        /// Process was not completed, but is no longer executed
        /// </summary>
        Interrupted,
        /// <summary>
        /// Process was completed successfully
        /// </summary>
        Completed,
    }

    /// <summary>
    /// Event args for process changes
    /// </summary>
    public class ProcessUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Process reference
        /// </summary>
        public IProcess Process { get; }

        /// <summary>
        ///  Current progress
        /// </summary>
        public ProcessProgress Progress { get; }

        /// <summary>
        /// Initialize a new event args instance
        /// </summary>
        public ProcessUpdatedEventArgs(IProcess process, ProcessProgress progress)
        {
            Process = process;
            Progress = progress;
        }
    }
}
