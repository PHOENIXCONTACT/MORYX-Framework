// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Event arguments for the job state change
    /// </summary>
    public class JobStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Job that changed state
        /// </summary>
        public Job Job { get; }

        /// <summary>
        /// Previous state of the job
        /// </summary>
        public JobClassification PreviousState { get; }

        /// <summary>
        /// Current state of the job
        /// </summary>
        public JobClassification CurrentState { get; }

        /// <summary>
        /// Create new event args
        /// </summary>
        public JobStateChangedEventArgs(Job job, JobClassification previousState, JobClassification currentState)
        {
            Job = job;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
