// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Public representation of production job
    /// </summary>
    internal interface IProductionJobData : IJobData
    {
        /// <summary>
        /// Current recipe of this job
        /// </summary>
        new ProductionRecipe Recipe { get; }

        /// <summary>
        /// Number of successful processes
        /// </summary>
        int SuccessCount { get; }

        /// <summary>
        /// Number of failed processes
        /// </summary>
        int FailureCount { get; }

        /// <summary>
        /// Number of processes that were predicted to fail
        /// </summary>
        int PredictedFailures { get; }

        /// <summary>
        /// Number of reworked processes
        /// </summary>
        int ReworkedCount { get; }

        /// <summary>
        /// Summary of running, success and failed processes
        /// </summary>
        int ProcessCount { get; }
    }
}
