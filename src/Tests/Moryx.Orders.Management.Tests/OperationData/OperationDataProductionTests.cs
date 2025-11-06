// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.ControlSystem.Jobs;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationDataProductionTests : OperationDataTestBase
    {
        [Test(Description = "If a job will be added to the operation, the updated event should be raised.")]
        public void AddJobRaisesUpdatedEvent()
        {
            // Arrange
            var operationData = InitializeOperationData(10, false, 11, 9);

            var updatedCalled = false;
            operationData.Updated += (_, _) => updatedCalled = true;

            // Act
            operationData.AddJob(new Job(new ProductRecipe(), 1));

            // Assert
            Assert.That(updatedCalled, "Updated event was not called");
            Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(1));
        }

        [TestCase(true, Description = "If the creation succeeds the operation should be startable.")]
        [TestCase(false, Description = "If the creation fails the operation should not be startable.")]
        public void BeginOperationAfterCreation(bool creationResult)
        {
            // Arrange
            var operationData = InitializeOperationData(10, false, 11, 9);
            operationData.Assign();

            // Act - Creation complete
            operationData.AssignCompleted(creationResult);

            var startedRaised = false;
            operationData.Started += (_, _) => startedRaised = true;

            // Assert
            // if creation failed, CanBegin: false
            // if creation success, CanBegin: true
            Assert.That(operationData.State.CanBegin, Is.EqualTo(creationResult));

            if (creationResult)
            {
                Assert.DoesNotThrow(() => operationData.Adjust(10, User));

                // We should have a job now
                Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(1));

                // Started should be raised
                Assert.That(startedRaised);

                // Check possible actions
                Assert.That(operationData.State.CanBegin);
                Assert.That(operationData.State.CanPartialReport);
                Assert.That(operationData.State.CanFinalReport, Is.False);
                Assert.That(operationData.State.CanInterrupt);
                Assert.That(operationData.State.CanAdvice);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => operationData.Adjust(10, User));
            }
        }

        [Test(Description = "Adjust an operation in state " + nameof(ReadyState) + "with an amount less than 0 is not allowed.")]
        public void Adjust_WithReadyOperationAndNegativeAmount_Throws() =>
            Assert.That(() => GetReadyOperation(10, false, 10, 10).Adjust(-1, User), Throws.InvalidOperationException);

        [Test(Description = "Adjust an operation in state " + nameof(InterruptedState) + "with an amount less than 0 is not allowed.")]
        public void Adjust_WithInterruptedOperationAndNegativeAmount_Throws() =>
            Assert.That(() => GetInterruptedOperation(10, false, 10, 10).Adjust(-1, User), Throws.InvalidOperationException);

        [Test(Description = "Adjust an operation in state " + nameof(AmountReachedState) + "with an amount less than 0 is not allowed.")]
        public void Adjust_WithAmountReachedOperationAndNegativeAmount_Throws() =>
            Assert.That(() => GetAmountReachedOperation(10, false, 10, 10).Adjust(-1, User), Throws.InvalidOperationException);

        [Test(Description = "Adjust an operation in state " + nameof(CompletedState) + "with an amount less than 0 is not allowed.")]
        public void Adjust_WithCompletedOperationAndNegativeAmount_Throws() =>
            Assert.That(() => GetCompletedOperation(10, false, 10, 10).Adjust(-1, User), Throws.InvalidOperationException);

        [Test(Description = "If an operation is running or interrupting the target " +
            "amount can be reduced. A new job with the residual amount should be dispatched.")]
        public void Adjust_WithNegativeAmountToOneRemaining_CompletesJobAndDispatchesNewForRemaining()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 10, 10);
            var operation = operationData.Operation;

            // Act
            operationData.Adjust(-2, User);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(operation.TargetAmount, Is.EqualTo(8), nameof(Operation.TargetAmount) + " should have been reduced.");
                Assert.That(() => JobHandlerMock.Verify(h => h.Complete(operationData), Times.Once), Throws.Nothing);
            });
        }

        [TestCase(9, 7, 2, 5, 0, 9, 1, Description = "Validate begin context without replace scrap: In this case, there are more rework than failure.")]
        [TestCase(8, 6, 2, 2, 0, 8, 2, Description = "Validate begin context without replace scrap: In this case, rework is equal to failure.")]
        [TestCase(8, 6, 2, 0, 2, 8, 2, Description = "Validate begin context without replace scrap: In this case, rework is less than failure.")]
        [TestCase(8, 0, 4, 2, 2, 8, 2, Description = "Validate begin context without replace scrap: In this case, rework failed.")]
        public void ValidateBeginContextWithoutReplaceScrap(int targetAmount, int success, int failure, int reworked, int expScrap, int expPartial, int expResidual)
        {
            // Arrange
            var operationData = GetReadyOperation(10, false, 10, 10);
            operationData.Adjust(targetAmount, User);

            var job = operationData.Operation.Jobs.First();
            job.Classification = JobClassification.Running;
            job.SuccessCount = success;
            job.FailureCount = failure;
            job.ReworkedCount = reworked;

            operationData.JobStateChanged(new JobStateChangedEventArgs(job, JobClassification.Idle, job.Classification));

            // Act
            var beginContext = operationData.GetBeginContext();

            // Assert
            Assert.That(beginContext.ScrapCount, Is.EqualTo(expScrap), "Scrap amount does not equal to" + expScrap);
            Assert.That(beginContext.PartialAmount, Is.EqualTo(expPartial), "Partial amount does not equal to " + expPartial);
            Assert.That(beginContext.ResidualAmount, Is.EqualTo(expResidual), "Residual amount does not equal to " + expResidual);
            Assert.That(beginContext.ResidualAmount + beginContext.PartialAmount, Is.EqualTo(10));
        }

        [TestCase(9, 9, 2, 5, 0, 9, 1, Description = "Validate begin context with replace scrap: In this case, there are more rework than failure.")]
        [TestCase(8, 8, 2, 2, 0, 8, 2, Description = "Validate begin context with replace scrap: In this case, rework is equal to failure.")]
        [TestCase(8, 8, 2, 0, 2, 8, 2, Description = "Validate begin context with replace scrap: In this case, rework is less than failure.")]
        public void ValidateBeginContextWithReplaceScrap(int amount, int success, int failure, int reworked, int expScrap, int expPartial, int expResidual)
        {
            // Arrange
            var operationData = GetReadyOperation(10, true, 10, 10);
            operationData.Adjust(amount, User);

            var firstJob = operationData.Operation.Jobs.First();
            operationData.JobStateChanged(new JobStateChangedEventArgs(firstJob, JobClassification.Idle, JobClassification.Running));
            // Produce the failure
            firstJob.Classification = JobClassification.Running;
            firstJob.SuccessCount = 0;
            firstJob.FailureCount = failure;
            firstJob.ReworkedCount = 0;
            operationData.JobProgressChanged(firstJob);

            // Produce the rework
            firstJob.Classification = JobClassification.Running;
            firstJob.SuccessCount = reworked;
            firstJob.FailureCount = failure;
            firstJob.ReworkedCount = reworked;
            operationData.JobProgressChanged(firstJob);

            // produce the success minus failure
            firstJob.Classification = JobClassification.Completing;
            firstJob.SuccessCount += success - failure - reworked;
            firstJob.FailureCount = failure;
            firstJob.ReworkedCount = reworked;
            operationData.JobProgressChanged(firstJob);
            operationData.JobStateChanged(new JobStateChangedEventArgs(firstJob, JobClassification.Completing, JobClassification.Completed));

            // Act - The beginContext should be as expected without completing the second job
            // We assume that the scrap parts will be replaced automatically
            var beginContext = operationData.GetBeginContext();

            // Assert
            Assert.That(beginContext.ScrapCount, Is.EqualTo(expScrap), "Scrap amount does not equal to" + expScrap);
            Assert.That(beginContext.PartialAmount, Is.EqualTo(expPartial), "Partial amount does not equal to " + expPartial);
            Assert.That(beginContext.ResidualAmount, Is.EqualTo(expResidual), "Residual amount does not equal to " + expResidual);
            Assert.That(beginContext.ResidualAmount + beginContext.PartialAmount, Is.EqualTo(10));

            // Arrange
            // complete the replace scrap job with the given failure count
            var secondJob = operationData.Operation.Jobs.Last();
            secondJob.Classification = JobClassification.Completed;
            secondJob.SuccessCount = failure;
            secondJob.FailureCount = 0;
            secondJob.ReworkedCount = 0;
            operationData.JobProgressChanged(secondJob);
            operationData.JobStateChanged(new JobStateChangedEventArgs(secondJob, JobClassification.Completing, JobClassification.Completed));

            // Act
            beginContext = operationData.GetBeginContext();

            // Assert
            Assert.That(beginContext.ScrapCount, Is.EqualTo(expScrap), "Scrap amount does not equal to" + expScrap);
            Assert.That(beginContext.PartialAmount, Is.EqualTo(expPartial), "Partial amount does not equal to " + expPartial);
            Assert.That(beginContext.ResidualAmount, Is.EqualTo(expResidual), "Residual amount does not equal to " + expResidual);
            Assert.That(beginContext.ResidualAmount + beginContext.PartialAmount, Is.EqualTo(10));
        }

        [Test(Description = "Validate begin context without replace scrap: In this case a job will be completed prematurely " +
                            "but should be considered in the begin context")]
        public void ValidateBeginContextWithPrematurelyCompletedJob()
        {
            // Arrange
            var operationData = GetReadyOperation(10, false, 10, 10);

            // Act
            operationData.Adjust(5, User);
            operationData.Adjust(5, User);

            var firstJob = operationData.Operation.Jobs.First();
            firstJob.Classification = JobClassification.Completed;

            var beginContext = operationData.GetBeginContext();

            // Assert
            Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(2));
            Assert.That(beginContext.ResidualAmount, Is.EqualTo(0));
            Assert.That(beginContext.PartialAmount, Is.EqualTo(10));
            Assert.That(beginContext.ResidualAmount + beginContext.PartialAmount, Is.EqualTo(10));
        }

        [Test(Description = "If a operation is interrupting, the worker should have the possibility " +
                            "to begin the operation again. A new job with the residual amount should be dispatched.")]
        public void BeginOperationWhileInterrupting()
        {
            // Arrange
            var operationData = GetInterruptingOperation(10, false, 10, 10);
            var operation = operationData.Operation;

            var firstJob = operation.Jobs.First();
            firstJob.Classification = JobClassification.Completing;
            firstJob.SuccessCount = 5;
            ((TestJob)firstJob).SetRunning(1);
            operationData.JobProgressChanged(firstJob);

            var startedRaised = false;
            operationData.Started += (_, _) => startedRaised = true;

            // Act
            operationData.Adjust(0, User);

            // Assert
            Assert.That(startedRaised, Is.False, "Started event should not be raised.");
            Assert.That(operation.Jobs.Count, Is.EqualTo(2), "No additional job should be dispatched after begin with no amount.");
            Assert.That(operationData.State.CanInterrupt);

            var secondJob = operation.Jobs.Last();
            Assert.That(secondJob.Amount, Is.EqualTo(4));
        }

        [Test(Description = "If a operation is interrupted, the worker should have the possibility " +
                            "to begin the operation again. A new job should be dispatched.")]
        public void BeginOperationWhileInterrupted()
        {
            // Arrange
            var operationData = GetInterruptedOperation(10, false, 10, 10);

            var startedRaised = false;
            operationData.Started += (_, _) => startedRaised = true;

            // Act
            operationData.Adjust(4, User);

            // Assert
            Assert.That(startedRaised);
            Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(2), "No job was dispatched after begin.");
        }

        [Test(Description = "If a operation is interrupted, the worker should have the possibility " +
                            "to begin the operation with an amount of zero to start the operation.")]
        public void BeginOperationWhileInterruptedWithAmountOfZero()
        {
            // Arrange
            var operationData = GetInterruptedOperation(10, false, 10, 10);

            var startedRaised = false;
            operationData.Started += (_, _) => startedRaised = true;

            // Act
            operationData.Adjust(0, User);

            // Assert
            Assert.That(startedRaised);
            Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(1), "No new job should be dispatched.");
        }

        [Test(Description = "Begin of an operation while it is completed should be not possible.")]
        public void BeginOperationWhileCompleted()
        {
            // Arrange
            var operationData = GetCompletedOperation(10, false, 10, 10);

            // Act - Assert
            Assert.That(operationData.State.CanBegin, Is.False);
            Assert.Throws<InvalidOperationException>(() => operationData.Adjust(1, User));
        }

        [Test(Description = "Will simulate reaching the operation amount with only success parts and do not replace scrap.")]
        public void ReachAmountWithSuccessParts()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 10, 10);

            // Act
            var job = operationData.Operation.Jobs.First();
            job.Classification = JobClassification.Completed;
            job.SuccessCount = job.Amount;

            operationData.JobStateChanged(new JobStateChangedEventArgs(job, JobClassification.Completing, JobClassification.Completed));

            // Assert
            // Check possible actions
            Assert.That(operationData.State.CanBegin);
            Assert.That(operationData.State.CanPartialReport);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(operationData.State.CanInterrupt);
            Assert.That(operationData.State.CanAdvice);
        }

        [Test(Description = "Will simulate reaching the operation amount with only failure parts and do not replace scrap.")]
        public void ReachAmountWithFailureParts()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 10, 10);

            // Act
            var job = operationData.Operation.Jobs.First();
            job.Classification = JobClassification.Completed;
            job.FailureCount = job.Amount;

            operationData.JobStateChanged(new JobStateChangedEventArgs(job, JobClassification.Completing, JobClassification.Completed));

            // Assert
            // Check possible actions
            Assert.That(operationData.State.CanBegin);
            Assert.That(operationData.State.CanPartialReport);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(operationData.State.CanInterrupt);
            Assert.That(operationData.State.CanAdvice);
        }

        [TestCase(3, 0, Description = "Will simulate replacing scrap if parts fail in completing job.")]
        [TestCase(0, 2, Description = "Will simulate replacing scrap if parts are predicted to fail in completing job.")]
        [TestCase(2, 1, Description = "Will simulate replacing scrap if parts failed and are predicted to fail in completing job.")]
        public void ReplaceScrapWhileRunning(int failure, int predictedFailure)
        {
            var targetAmount = 10;
            // Arrange
            var operationData = GetRunningOperation(targetAmount, true, 10, 10);

            // Act
            var initialJob = operationData.Operation.Jobs.First();
            initialJob.Classification = JobClassification.Completing;
            initialJob.SuccessCount = 6;
            initialJob.FailureCount = failure;
            // RunningProcesses containing the predicted fauilure, too.
            ((TestJob)initialJob).SetRunning(1 + predictedFailure);
            ((TestJob)initialJob).PredictedFailures = Enumerable.Range(1, predictedFailure)
                .Select(i => new Process { Id = i }).ToList();

            operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Running, JobClassification.Completing));

            // Assert
            Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(2));

            // The additional job should replace the Failure- and the PredictedFailureCount
            var scrapJob = operationData.Operation.Jobs.First(j => j != initialJob);

            // Calculate the missing amount: target - already success - running those are not failing
            // Adding predicted Failure because all predictedFailure are Runnings , but not all runnings are predictedFailure.
            Assert.That(scrapJob.Amount, Is.EqualTo(targetAmount - initialJob.SuccessCount - initialJob.RunningProcesses.Count + predictedFailure));
        }

        [Test(Description = "Will simulate replacing scrap if a part fails in completing scrap job.")]
        public void ReplaceScrapOfScrapJobWhileRunning()
        {
            // Arrange
            var operationData = GetRunningOperation(10, true, 10, 10);

            // Act
            var initialJob = operationData.Operation.Jobs.First();
            initialJob.Classification = JobClassification.Completing;
            initialJob.SuccessCount = 6;
            ((TestJob)initialJob).SetRunning(1);
            initialJob.FailureCount = 3;

            operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Running, JobClassification.Completing));

            initialJob.Classification = JobClassification.Completed;
            initialJob.SuccessCount = 7;
            ((TestJob)initialJob).SetRunning(0);
            initialJob.FailureCount = 3;

            operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Completing, JobClassification.Completed));

            var scrapJob = operationData.Operation.Jobs.Last();

            scrapJob.Classification = JobClassification.Completing;
            scrapJob.SuccessCount = 2;
            ((TestJob)scrapJob).SetRunning(1);
            scrapJob.FailureCount = 0;

            operationData.JobStateChanged(new JobStateChangedEventArgs(scrapJob, JobClassification.Running, JobClassification.Completing));

            scrapJob.Classification = JobClassification.Completed;
            scrapJob.SuccessCount = 2;
            ((TestJob)scrapJob).SetRunning(0);
            scrapJob.FailureCount = 1;

            operationData.JobStateChanged(new JobStateChangedEventArgs(scrapJob, JobClassification.Completing, JobClassification.Completed));

            // Assert
            // Now we should have three jobs
            Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(3));

            var secondScrapJob = operationData.Operation.Jobs.Last();
            Assert.That(secondScrapJob.Amount, Is.EqualTo(1));
        }

        [Test(Description = "Will interrupt a running operation. Jobs should be complete.")]
        public void InterruptWhileRunning()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 10, 10);

            var partialReportRaised = false;
            operationData.PartialReport += (_, _) => partialReportRaised = true;

            var report = new OperationReport(ConfirmationType.Partial, 0, 0, User);

            // Act
            operationData.Interrupt(report);

            // Assert
            // We should now be interrupting
            JobHandlerMock.Verify(d => d.Complete(operationData), Times.Once);
            Assert.That(partialReportRaised);
            Assert.That(operationData.State.CanBegin);
            Assert.That(operationData.State.CanPartialReport);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.That(operationData.State.CanInterrupt, Is.False);
            Assert.That(operationData.State.CanAdvice);
        }

        [Test(Description = "Operation should be interrupted if all jobs are completed while interrupting.")]
        public void JobCompletionWhileInterrupting()
        {
            // Arrange
            var operationData = GetInterruptingOperation(10, false, 10, 10);
            var initialJob = operationData.Operation.Jobs.First();
            var report = new OperationReport(ConfirmationType.Partial, 0, 0, User);

            // Act
            initialJob.SuccessCount = initialJob.Amount;
            initialJob.Classification = JobClassification.Completed;

            operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Idle, JobClassification.Completed));

            // Assert
            Assert.That(operationData.State.CanInterrupt, Is.False);
            Assert.Throws<InvalidOperationException>(() => operationData.Interrupt(report));
        }
    }
}

