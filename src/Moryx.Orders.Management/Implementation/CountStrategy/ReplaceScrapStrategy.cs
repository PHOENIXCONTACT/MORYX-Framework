// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// class to use if we have to replace the scrap
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(ICountStrategy), Name = StrategyName)]
    internal class ReplaceScrapStrategy : ICountStrategy
    {
        public const string StrategyName = "ReplaceScrap";

        /// <inheritdoc />
        public IEnumerable<Job> RelevantJobs(Operation operation)
        {
            return operation.Jobs;
        }

        /// <inheritdoc />
        public bool AmountReached(Operation operation)
        {
            return operation.Progress.SuccessCount >= operation.TargetAmount;
        }

        /// <inheritdoc />
        public bool CanReachAmount(Operation operation)
        {
            // You can get it if you really want
            // But you must try, try and try, try and try
            // You'll succeed at last, mmh, yeah (Jimmy Cliff)
            // --> Failure have to be success
            return ReachableAmount(operation) >= operation.TargetAmount;
        }

        /// <inheritdoc />
        public int ReachableAmount(Operation operation)
        {
            return operation.Jobs.Where(j => j.Classification < JobClassification.Completed)
                .Aggregate(operation.Progress.SuccessCount, Reachable);
        }

        private static int Reachable(int sum, Job job)
        {
            if (job.Classification >= JobClassification.Completing)
                sum = sum + job.RunningProcesses.Count;
            else
                sum = sum + job.Amount - job.SuccessCount - job.FailureCount;

            if (job is IPredictiveJob predictive)
                sum -= predictive.PredictedFailures.Count;
            return sum;
        }

        public IReadOnlyList<DispatchContext> MissingAmounts(Operation operation)
        {
            var missingAmount = operation.TargetAmount - ReachableAmount(operation);
            return missingAmount > 0
                ? [new DispatchContext((ProductionRecipe)operation.Recipes.Single(), (uint)missingAmount)]
                : Array.Empty<DispatchContext>();
        }
    }
}

