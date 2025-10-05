// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.ControlSystem.Jobs;

namespace Moryx.Orders
{
    /// <summary>
    /// Defines the replace scrap strategy
    /// </summary>
    public interface ICountStrategy
    {
        /// <summary>
        /// Returns the relevant jobs for the production parts
        /// </summary>
        IEnumerable<Job> RelevantJobs(Operation operation);

        /// <summary>
        /// Flag if operation have reached the amount.
        /// </summary>
        bool AmountReached(Operation operation);

        /// <summary>
        /// Flag if the operation can reach the amount with the current jobs
        /// </summary>
        bool CanReachAmount(Operation operation);

        /// <summary>
        /// Sum of SuccessCount and RunningCount of jobs. All running will be classified as "success"
        /// </summary>
        int ReachableAmount(Operation operation);

        /// <summary>
        /// Returns the amounts which are currently missing
        /// </summary>
        IReadOnlyList<DispatchContext> MissingAmounts(Operation operation);
    }
}