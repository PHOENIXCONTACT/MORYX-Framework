// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Optional extension interface for <see cref="Job"/>
    /// </summary>
    public interface IPredictiveJob
    {
        /// <summary>
        /// Processes that are predicted to fail based on workplan and current progress
        /// </summary>
        IReadOnlyList<IProcess> PredictedFailures { get; }
    }
}
