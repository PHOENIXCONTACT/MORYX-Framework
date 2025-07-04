// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Public representation of the job state
    /// </summary>
    internal interface IJobState
    {
        /// <summary>
        /// Key of the job state
        /// </summary>
        int Key { get; }

        /// <summary>
        /// Classification of the job
        /// </summary>
        JobClassification Classification { get; }
    }
}
