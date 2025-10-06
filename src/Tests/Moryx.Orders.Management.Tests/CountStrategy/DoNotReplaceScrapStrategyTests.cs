// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.TestTools;
using Moryx.Logging;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class DoNotReplaceScrapStrategyTests
    {
        private DoNotReplaceScrapStrategy _countStrategy;

        [SetUp]
        public void SetUp()
        {
            _countStrategy = new DoNotReplaceScrapStrategy();
        }

        [Description("Do not return missing amounts even if scrap parts were reworked")]
        [TestCase(0, 4)]
        [TestCase(4, 0)]
        public void NoMissingAmountIfPartsReworked(int failureCount, int runningCount)
        {
            // Arrange
            var operationData = CreateOperationData();
            var operation = operationData.Operation;

            operation.TotalAmount = 10;
            operationData.AddJob(new OperationDataTestBase.TestJob(new ProductRecipe(), 10)
            {
                Classification = JobClassification.Completing,
                SuccessCount = 6,
                ReworkedCount = 6,
                FailureCount = failureCount,
            }.SetRunning(runningCount));

            // Act
            var missingAmounts = _countStrategy.MissingAmounts(operation);

            // Assert
            Assert.That(missingAmounts.Count, Is.EqualTo(0), "There should no missing amount!");
        }

        [TestCase(10, 7, 2, 5, 0, JobClassification.Completed, 9, false, false, 1, 0, Description = "Validate data: In this case, there are more rework than failure.")]
        [TestCase(10, 6, 2, 2, 0, JobClassification.Completed, 8, false, false, 2, 0, Description = "Validate data: In this case, rework is equal to failure.")]
        [TestCase(10, 6, 2, 0, 0, JobClassification.Completed, 8, false, false, 2, 0, Description = "Validate data: In this case, rework is less than failure.")]
        [TestCase(10, 0, 4, 2, 0, JobClassification.Completed, 4, false, false, 6, 0, Description = "Validate data: In this case, rework failed.")]
        [TestCase(10, 10, 0, 5, 0, JobClassification.Completed, 10, true, true, 0, 0, Description = "Validate data: In case that there are no scraps but reworked parts")]
        [TestCase(10, 10, 0, 10, 0, JobClassification.Completed, 10, true, true, 0, 0, Description = "Validate data: In case that there are no scraps but all parts are reworked")]
        [TestCase(10, 6, 2, 0, 2, JobClassification.Running, 10, true, false, 0, 2, Description = "Validate data: Production without reworked parts")]
        [TestCase(10, 9, 0, 5, 0, JobClassification.Running, 10, true, false, 0, 1, Description = "Validate data: Production with reworked parts but no scrap")]
        [TestCase(10, 9, 0, 9, 0, JobClassification.Running, 10, true, false, 0, 1, Description = "Validate data: Production with completely reworked parts")]
        [TestCase(10, 6, 2, 0, 2, JobClassification.Completing, 8, false, false, 2, 0, Description = "Validate data: Completing production without reworked parts")]
        [TestCase(10, 9, 0, 5, 1, JobClassification.Completing, 9, false, false, 1, 0, Description = "Validate data: Completing production with reworked parts but no scrap")]
        [TestCase(10, 9, 0, 9, 1, JobClassification.Completing, 9, false, false, 1, 0, Description = "Validate data: Completing production with completely reworked parts")]
        public void ValidateData(int targetAmount, int success, int failure, int reworked, int running, JobClassification classification,
            int expReachable, bool expCanReach, bool expReached, int expMissing, int expPending)
        {
            // Arrange
            var operationData = CreateOperationData();
            var operation = operationData.Operation;

            operation.TargetAmount = targetAmount;
            operationData.AddJob(new OperationDataTestBase.TestJob(new ProductRecipe(), targetAmount)
            {
                Classification = classification,
                SuccessCount = success,
                FailureCount = failure,
                ReworkedCount = reworked
            });

            // Act
            var reachable = _countStrategy.ReachableAmount(operation);
            var canReach = _countStrategy.CanReachAmount(operation);
            var reached = _countStrategy.AmountReached(operation);
            var missing = _countStrategy.MissingAmounts(operation);

            // Assert
            Assert.That(reachable, Is.EqualTo(expReachable));
            Assert.That(canReach, Is.EqualTo(expCanReach));
            Assert.That(reached, Is.EqualTo(expReached));
            if (expMissing > 0)
            {
                Assert.That(missing.First().Amount, Is.EqualTo(expMissing));
            }
            else
            {
                Assert.That(missing.Count, Is.EqualTo(0));
            }

            Assert.That(operationData.Operation.Progress.PendingCount, Is.EqualTo(expPending));
        }

        private OperationData CreateOperationData()
        {
            var operationData = new OperationData
            {
                OrderData = new OrderData(),
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                CountStrategy = _countStrategy,
            };

            operationData.Initialize(new OperationCreationContext(), new OrderData(), new NullOperationSource());
            operationData.Operation.Recipes.Add(new DummyRecipe());
            return operationData;
        }
    }
}

