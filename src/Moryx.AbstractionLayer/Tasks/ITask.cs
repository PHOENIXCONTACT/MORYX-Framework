// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Task interface
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Id of the task
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Type of activity created by this task
        /// </summary>
        Type ActivityType { get; }

        /// <summary>
        /// Creates a new IActivity object
        /// </summary>
        /// <returns>the new IActivity object</returns>
        /// <exception cref="ArgumentException">if the params object's type does not match to activity.</exception>
        IActivity CreateActivity(IProcess process);

        /// <summary>
        /// Complete task
        /// </summary>
        void Completed(ActivityResult result);
    }
}
