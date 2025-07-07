// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Cells;

namespace Moryx.Resources.Benchmarking
{
    /// <summary>
    /// Interface for all benchmark resources
    /// </summary>
    public interface IBenchmarkResource : ICell
    {
        /// <summary>
        /// Get activity count since last call
        /// </summary>
        BenchmarkReport GetReport();
    }

    /// <summary>
    /// Report of current performance in the resource
    /// </summary>
    public struct BenchmarkReport
    {
        /// <summary>
        /// Total number of activities
        /// </summary>
        public int ActivityCount;

        /// <summary>
        /// Time spent waiting for a ReadyToWork response
        /// </summary>
        public long ReadyToWorkWait;

        /// <summary>
        /// Time spent waiting for an ActivityCompleted response
        /// </summary>
        public long ActivityCompletionWait;
    }
}
