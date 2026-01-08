// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;

namespace Moryx.ControlSystem.Activities
{
    /// <summary>
    /// Interface for activity parameters to define a timeout for the kernel
    /// </summary>
    public interface IActivityTimeoutParameters : IParameters
    {
        /// <summary>
        /// Timeout in seconds for this activity
        /// </summary>
        int Timeout { get; }
    }
}
