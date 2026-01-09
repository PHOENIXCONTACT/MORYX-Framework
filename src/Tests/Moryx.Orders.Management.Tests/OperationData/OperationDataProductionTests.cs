// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Threading.Tasks;
using Moryx.ControlSystem.Jobs;
using Moq;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests;

[TestFixture]
public class OperationDataProductionTests : OperationDataTestBase
{
    [Test(Description = "If a job will be added to the operation, the updated event should be raised.")]
    public async Task AddJobRaisesUpdatedEvent()
    {
        // Arrange
        var operationData = await InitializeOperationData(10, false, 11, 9);

        var updatedCalled = false;
        operationData.Updated += (_, _) => updatedCalled = true;

        // Act
        await operationData.AddJob(new Job(new ProductRecipe(), 1));

        // Assert
        Assert.That(updatedCalled, "Updated event was not called");
        Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(1));
    }

    [TestCase(true, Description = "If the creation succeeds the operation should be startable.")]
    [TestCase(false, Description = "If the creation fails the operation should not be startable.")]
    public async Task BeginOperationAfterCreation(bool creationResult)
    {
        // Arrange
        var operationData = await InitializeOperationData(10, false, 11, 9);
        await operationData.Assign();

        // Act - Creation complete
        await operationData.AssignCompleted(creationResult);

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
            Assert.ThrowsAsync<InvalidOperationException>(() => operationData.Adjust(10, User));
        }
    }

    [Test(Description = "Adjust an operation in state " + nameof(ReadyState) + "with an amount less than 0 is not allowed.")]
    public async Task Adjust_WithReadyOperationAndNegativeAmount_Throws()
    {
        // Arrange
        var operation = await GetReadyOperation(10, false, 10, 10);

        // Act / Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => operation.Adjust(-1, User));
    }

    [Test(Description = "Adjust an operation in state " + nameof(InterruptedState) + "with an amount less than 0 is not allowed.")]
    public async Task Adjust_WithInterruptedOperationAndNegativeAmount_Throws()
    {
        // Arrange
        var operation = await GetInterruptedOperation(10, false, 10, 10);

        // Act / Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => operation.Adjust(-1, User));
    }

    [Test(Description = "Adjust an operation in state " + nameof(AmountReachedState) + "with an amount less than 0 is not allowed.")]
    public async Task Adjust_WithAmountReachedOperationAndNegativeAmount_Throws()
    {
        // Arrange
        var operation = await GetAmountReachedOperation(10, false, 10, 10);

        // Act / Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => operation.Adjust(-1, User));
    }

    [Test(Description = "Adjust an operation in state " + nameof(CompletedState) + "with an amount less than 0 is not allowed.")]
    public async Task Adjust_WithCompletedOperationAndNegativeAmount_Throws()
    {
        // Arrange
        var operation = await GetAmountReachedOperation(10, false, 10, 10);

        // Act / Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => operation.Adjust(-1, User));
    }

    [Test(Description = "If an operation is running or interrupting the target " +
                        "amount can be reduced. A new job with the residual amount should be dispatched.")]
    public async Task  Adjust_WithNegativeAmountToOneRemaining_CompletesJobAndDispatchesNewForRemaining()
    {
        // Arrange
        var operationData = await GetRunningOperation(10, false, 10, 10);
        var operation = operationData.Operation;

        // Act
        await operationData.Adjust(-2, User);

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
    public async Task  ValidateBeginContextWithoutReplaceScrap(int targetAmount, int success, int failure, int reworked, int expScrap, int expPartial, int expResidual)
    {
        // Arrange
        var operationData = await GetReadyOperation(10, false, 10, 10);
        await operationData.Adjust(targetAmount, User);

        var job = operationData.Operation.Jobs.First();
        job.Classification = JobClassification.Running;
        job.SuccessCount = success;
        job.FailureCount = failure;
        job.ReworkedCount = reworked;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(job, JobClassification.Idle, job.Classification));

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
    public async Task  ValidateBeginContextWithReplaceScrap(int amount, int success, int failure, int reworked, int expScrap, int expPartial, int expResidual)
    {
        // Arrange
        var operationData = await GetReadyOperation(10, true, 10, 10);
        await operationData.Adjust(amount, User);

        var firstJob = operationData.Operation.Jobs.First();
        await operationData.JobStateChanged(new JobStateChangedEventArgs(firstJob, JobClassification.Idle, JobClassification.Running));
        // Produce the failure
        firstJob.Classification = JobClassification.Running;
        firstJob.SuccessCount = 0;
        firstJob.FailureCount = failure;
        firstJob.ReworkedCount = 0;
        await operationData.JobProgressChanged(firstJob);

        // Produce the rework
        firstJob.Classification = JobClassification.Running;
        firstJob.SuccessCount = reworked;
        firstJob.FailureCount = failure;
        firstJob.ReworkedCount = reworked;
        await operationData.JobProgressChanged(firstJob);

        // produce the success minus failure
        firstJob.Classification = JobClassification.Completing;
        firstJob.SuccessCount += success - failure - reworked;
        firstJob.FailureCount = failure;
        firstJob.ReworkedCount = reworked;
        await operationData.JobProgressChanged(firstJob);
        await operationData.JobStateChanged(new JobStateChangedEventArgs(firstJob, JobClassification.Completing, JobClassification.Completed));

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
        await operationData.JobProgressChanged(secondJob);
        await operationData.JobStateChanged(new JobStateChangedEventArgs(secondJob, JobClassification.Completing, JobClassification.Completed));

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
    public async Task  ValidateBeginContextWithPrematurelyCompletedJob()
    {
        // Arrange
        var operationData = await GetReadyOperation(10, false, 10, 10);

        // Act
        await operationData.Adjust(5, User);
        await operationData.Adjust(5, User);

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
    public async Task  BeginOperationWhileInterrupting()
    {
        // Arrange
        var operationData = await GetInterruptingOperation(10, false, 10, 10);
        var operation = operationData.Operation;

        var firstJob = operation.Jobs.First();
        firstJob.Classification = JobClassification.Completing;
        firstJob.SuccessCount = 5;
        ((TestJob)firstJob).SetRunning(1);
        await operationData.JobProgressChanged(firstJob);

        var startedRaised = false;
        operationData.Started += (_, _) => startedRaised = true;

        // Act
        await operationData.Adjust(0, User);

        // Assert
        Assert.That(startedRaised, Is.False, "Started event should not be raised.");
        Assert.That(operation.Jobs.Count, Is.EqualTo(2), "No additional job should be dispatched after begin with no amount.");
        Assert.That(operationData.State.CanInterrupt);

        var secondJob = operation.Jobs.Last();
        Assert.That(secondJob.Amount, Is.EqualTo(4));
    }

    [Test(Description = "If a operation is interrupted, the worker should have the possibility " +
                        "to begin the operation again. A new job should be dispatched.")]
    public async Task  BeginOperationWhileInterrupted()
    {
        // Arrange
        var operationData = await GetInterruptedOperation(10, false, 10, 10);

        var startedRaised = false;
        operationData.Started += (_, _) => startedRaised = true;

        // Act
        await operationData.Adjust(4, User);

        // Assert
        Assert.That(startedRaised);
        Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(2), "No job was dispatched after begin.");
    }

    [Test(Description = "If a operation is interrupted, the worker should have the possibility " +
                        "to begin the operation with an amount of zero to start the operation.")]
    public async Task  BeginOperationWhileInterruptedWithAmountOfZero()
    {
        // Arrange
        var operationData = await GetInterruptedOperation(10, false, 10, 10);

        var startedRaised = false;
        operationData.Started += (_, _) => startedRaised = true;

        // Act
        await operationData.Adjust(0, User);

        // Assert
        Assert.That(startedRaised);
        Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(1), "No new job should be dispatched.");
    }

    [Test(Description = "Begin of an operation while it is completed should be not possible.")]
    public async Task BeginOperationWhileCompleted()
    {
        // Arrange
        var operationData = await GetCompletedOperation(10, false, 10, 10);

        // Act - Assert
        Assert.That(operationData.State.CanBegin, Is.False);
        Assert.ThrowsAsync<InvalidOperationException>(() => operationData.Adjust(1, User));
    }

    [Test(Description = "Will simulate reaching the operation amount with only success parts and do not replace scrap.")]
    public async Task ReachAmountWithSuccessParts()
    {
        // Arrange
        var operationData = await GetRunningOperation(10, false, 10, 10);

        // Act
        var job = operationData.Operation.Jobs.First();
        job.Classification = JobClassification.Completed;
        job.SuccessCount = job.Amount;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(job, JobClassification.Completing, JobClassification.Completed));

        // Assert
        // Check possible actions
        Assert.That(operationData.State.CanBegin);
        Assert.That(operationData.State.CanPartialReport);
        Assert.That(operationData.State.CanFinalReport);
        Assert.That(operationData.State.CanInterrupt);
        Assert.That(operationData.State.CanAdvice);
    }

    [Test(Description = "Will simulate reaching the operation amount with only failure parts and do not replace scrap.")]
    public async Task ReachAmountWithFailureParts()
    {
        // Arrange
        var operationData = await GetRunningOperation(10, false, 10, 10);

        // Act
        var job = operationData.Operation.Jobs.First();
        job.Classification = JobClassification.Completed;
        job.FailureCount = job.Amount;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(job, JobClassification.Completing, JobClassification.Completed));

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
    public async Task  ReplaceScrapWhileRunning(int failure, int predictedFailure)
    {
        var targetAmount = 10;
        // Arrange
        var operationData = await GetRunningOperation(targetAmount, true, 10, 10);

        // Act
        var initialJob = operationData.Operation.Jobs.First();
        initialJob.Classification = JobClassification.Completing;
        initialJob.SuccessCount = 6;
        initialJob.FailureCount = failure;
        // RunningProcesses containing the predicted fauilure, too.
        ((TestJob)initialJob).SetRunning(1 + predictedFailure);
        ((TestJob)initialJob).PredictedFailures = Enumerable.Range(1, predictedFailure)
            .Select(i => new Process { Id = i }).ToList();

        await operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Running, JobClassification.Completing));

        // Assert
        Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(2));

        // The additional job should replace the Failure- and the PredictedFailureCount
        var scrapJob = operationData.Operation.Jobs.First(j => j != initialJob);

        // Calculate the missing amount: target - already success - running those are not failing
        // Adding predicted Failure because all predictedFailure are Runnings , but not all runnings are predictedFailure.
        Assert.That(scrapJob.Amount, Is.EqualTo(targetAmount - initialJob.SuccessCount - initialJob.RunningProcesses.Count + predictedFailure));
    }

    [Test(Description = "Will simulate replacing scrap if a part fails in completing scrap job.")]
    public async Task  ReplaceScrapOfScrapJobWhileRunning()
    {
        // Arrange
        var operationData = await GetRunningOperation(10, true, 10, 10);

        // Act
        var initialJob = operationData.Operation.Jobs.First();
        initialJob.Classification = JobClassification.Completing;
        initialJob.SuccessCount = 6;
        ((TestJob)initialJob).SetRunning(1);
        initialJob.FailureCount = 3;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Running, JobClassification.Completing));

        initialJob.Classification = JobClassification.Completed;
        initialJob.SuccessCount = 7;
        ((TestJob)initialJob).SetRunning(0);
        initialJob.FailureCount = 3;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Completing, JobClassification.Completed));

        var scrapJob = operationData.Operation.Jobs.Last();

        scrapJob.Classification = JobClassification.Completing;
        scrapJob.SuccessCount = 2;
        ((TestJob)scrapJob).SetRunning(1);
        scrapJob.FailureCount = 0;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(scrapJob, JobClassification.Running, JobClassification.Completing));

        scrapJob.Classification = JobClassification.Completed;
        scrapJob.SuccessCount = 2;
        ((TestJob)scrapJob).SetRunning(0);
        scrapJob.FailureCount = 1;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(scrapJob, JobClassification.Completing, JobClassification.Completed));

        // Assert
        // Now we should have three jobs
        Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(3));

        var secondScrapJob = operationData.Operation.Jobs.Last();
        Assert.That(secondScrapJob.Amount, Is.EqualTo(1));
    }

    [Test(Description = "Will interrupt a running operation. Jobs should be complete.")]
    public async Task  InterruptWhileRunning()
    {
        // Arrange
        var operationData = await GetRunningOperation(10, false, 10, 10);

        // Act
        await operationData.Interrupt(User);

        // Assert
        // We should now be interrupting
        JobHandlerMock.Verify(d => d.Complete(operationData), Times.Once);
        Assert.That(operationData.State.CanBegin);
        Assert.That(operationData.State.CanPartialReport);
        Assert.That(operationData.State.CanFinalReport, Is.False);
        Assert.That(operationData.State.CanInterrupt, Is.False);
        Assert.That(operationData.State.CanAdvice);
    }

    [Test(Description = "Operation should be interrupted if all jobs are completed while interrupting.")]
    public async Task  JobCompletionWhileInterrupting()
    {
        // Arrange
        var operationData = await GetInterruptingOperation(10, false, 10, 10);
        var initialJob = operationData.Operation.Jobs.First();

        // Act
        initialJob.SuccessCount = initialJob.Amount;
        initialJob.Classification = JobClassification.Completed;

        await operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Idle, JobClassification.Completed));

        // Assert
        Assert.That(operationData.State.CanInterrupt, Is.False);
        Assert.ThrowsAsync<InvalidOperationException>(() => operationData.Interrupt(User));
    }
}