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
    internal class DoNotReplaceScrapStrategy : ICountStrategy
    {
        public const string StrategyName = "DoNotReplaceScrap";

        /// <inheritdoc />
        public IEnumerable<Job> RelevantJobs(Operation operation)
        {
            return operation.Jobs;
        }

        /// <inheritdoc />
        public bool AmountReached(Operation operation)
        {
            return operation.Progress.SuccessCount + operation.Progress.FailureCount >= operation.TargetAmount;
        }

        /// <inheritdoc />
        public bool CanReachAmount(Operation operation)
        {
            return ReachableAmount(operation) >= operation.TargetAmount;
        }

        /// <inheritdoc />
        public int ReachableAmount(Operation operation)
        {
            // Alle machen Fehler. Keiner ist ein Supermann.
            // Alle machen Fehler, weil das mal passieren kann. (Rolf Zuckowski)
            var running = operation.Jobs.Where(j => j.Classification != JobClassification.Completed)
                    .Sum(j => j.RunningProcesses.Count);

            var notStarted = operation.Jobs.Where(j => j.Classification < JobClassification.Completing)
                    .Sum(j => j.Amount - j.SuccessCount - j.RunningProcesses.Count - j.FailureCount);

            // --> Failures are okay and can be added to the reachable amount
            return operation.Progress.SuccessCount + running + notStarted + operation.Progress.FailureCount;
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

