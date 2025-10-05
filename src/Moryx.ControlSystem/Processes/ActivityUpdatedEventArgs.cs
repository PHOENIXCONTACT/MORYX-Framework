// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Three stages of an activity within the kernel 
    /// </summary>
    public enum ActivityProgress
    {
        /// <summary>
        /// Activity was prepared and is awaiting execution
        /// </summary>
        Ready,
        /// <summary>
        /// Activity was started in a cell
        /// </summary>
        Running,
        /// <summary>
        /// Activity was completed
        /// </summary>
        Completed
    }

    /// <summary>
    /// Event args for activity changes
    /// </summary>
    public class ActivityUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Activity references
        /// </summary>
        public IActivity Activity { get; }

        /// <summary>
        /// Current progress
        /// </summary>
        public ActivityProgress Progress { get; }

        /// <summary>
        /// Initialize the activity event args
        /// </summary>
        public ActivityUpdatedEventArgs(IActivity activity, ActivityProgress progress)
        {
            Activity = activity;
            Progress = progress;
        }
    }
}
