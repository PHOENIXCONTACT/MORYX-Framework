// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moryx.ControlSystem.Jobs;
using Moq;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Orders.Management.Model;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationDataRestoreTests : OperationDataTestBase
    {
        private IUnitOfWorkFactory<OrdersContext> _orderModel;
        private OrderData _orderData;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _orderModel = new UnitOfWorkFactory<OrdersContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));
            _orderData = new OrderData();
            _orderData.Order.Number = "30055221553";
        }

        [Test(Description = "No resume of creating operations. " +
                            "All tests should fail because creating operations should not be stored.")]
        public async Task ResumeCreating()
        {
            // Arrange
            var operationToSave = await InitializeOperationData(10, false, 11, 9);
            var databaseId = await SaveToDatabase(operationToSave);

            // Act - Assert: Initial
            var operationData = await ReloadFromDatabase(databaseId, false);
            await operationData.Restore();

            Assert.ThrowsAsync<InvalidOperationException>(() => operationData.Resume(), "There should be no restoring of Initial OperationData");

            // Act - Assert: Creating
            await operationData.Assign();
            Assert.ThrowsAsync<InvalidOperationException>(() => operationData.Resume(), "There should be no restoring of Creating OperationData");

            // Act - Assert: CreationFailed
            await operationData.AssignCompleted(false);
            Assert.ThrowsAsync<InvalidOperationException>(() => operationData.Resume(), "There should be no restoring of CreationFailed OperationData");
        }

        [Test(Description = "Restores a ready operation.")]
        public async Task RestoreReady()
        {
            // Arrange
            var operationToSave = await GetReadyOperation(10, false, 11, 9);
            var databaseId = await SaveToDatabase(operationToSave);

            // Act
            var operationData = await ReloadFromDatabase(databaseId, false);
            await operationData.Restore();

            //
            Assert.DoesNotThrow(() => AssignmentMock.Verify(c => c.Restore(operationData), Times.Once));
            Assert.DoesNotThrow(() => JobHandlerMock.Verify(c => c.Restore(It.IsAny<IEnumerable<long>>()), Times.Once));
            Assert.That(operationData.TotalAmount, Is.EqualTo(10));
            Assert.That(operationData.Operation.OverDeliveryAmount, Is.EqualTo(11));
            Assert.That(operationData.Operation.UnderDeliveryAmount, Is.EqualTo(9));
            Assert.That(operationData.Operation.Jobs.Count, Is.EqualTo(0));
            Assert.That(operationData.State.CanBegin);
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(operationData.State.CanAdvice);
            Assert.That(operationData.State.CanInterrupt, Is.False);
            if (operationData.Operation.Start != null)
                Assert.That(operationData.Operation.Start?.Kind, Is.EqualTo(DateTimeKind.Utc));
            if (operationData.Operation.End != null)
                Assert.That(operationData.Operation.End?.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(operationData.Operation.PlannedStart.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(operationData.Operation.PlannedEnd.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test(Description = "Restore a running operation should be possible." +
                            "Will simulate a production, stores the operation and reloads it after job was finished. " +
                            "The amount should be reached")]
        public async Task RestoreRunning()
        {
            // Arrange
            var operationData = await GetRunningOperation(10, false, 11, 9);

            // Simulate shutdown and with producing job
            var databaseId = await SimulateProductionWithShutdown(operationData);

            // Act
            operationData = await ReloadFromDatabase(databaseId, false);
            await operationData.Restore();
            await operationData.Resume();

            // because of the successful job, the restore
            // evaluates the jobs again and the amount should be reached

            // Assert
            await AssertAmountReached(operationData);
        }

        [Test(Description = "Restore a running operation. " +
                            "Will simulate a production, stores the operation and reloads it. " +
                            "Will not call Resume since no other job should be dispatched.")]
        public async Task DoNotDispatchJobAfterRestore()
        {
            // Arrange
            var operationData = await GetRunningOperation(10, false, 11, 10);

            // Simulate shutdown and with producing job
            var databaseId = await SimulateProductionWithShutdown(operationData);
            JobHandlerMock.Invocations.Clear();

            // Change the amount to not have amount reached and a new job can be created to go on with the production.
            var initialJob = operationData.Operation.Jobs.First();
            initialJob.SuccessCount = 8;

            // Act
            operationData = await ReloadFromDatabase(databaseId, false);
            await operationData.Restore();
            // Don't call operationData.Resume to be sure that no additional job will be dispatched.

            // Assert
            JobHandlerMock.Verify(d => d.Dispatch(It.IsAny<IOperationData>(), It.IsAny<IReadOnlyList<DispatchContext>>()), Times.Never);
        }

        [Test(Description = "Will restore an operation which is interrupting. " +
                            "After the restore, the operation should be interrupted.")]
        public async Task RestoreInterrupting()
        {
            // Arrange
            var operationData = await GetInterruptingOperation(10, false, 11, 9);

            // Simulate shutdown and with producing job
            var databaseId = await SimulateProductionWithShutdown(operationData);

            // Act
            operationData = await ReloadFromDatabase(databaseId, false);

            // because of the successful job, the restore
            // evaluates the jobs again and the operation should be amount reached

            var interruptedRaised = false;
            operationData.Interrupted += (_, _) => interruptedRaised = true;

            await operationData.Restore();
            await operationData.Resume();

            // Assert
            Assert.That(interruptedRaised, $"The {nameof(IOperationData.Interrupted)} event was not raised.");
            Assert.That(operationData.State.CanBegin);
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(operationData.State.CanInterrupt, Is.False);
            Assert.That(operationData.State.CanAdvice);
        }

        [Test(Description = "Will restore an operation which is interrupted.")]
        public async Task RestoreInterrupted()
        {
            // Arrange
            var operationData = await GetInterruptedOperation(10, false, 11, 9);

            // Simulate shutdown and with producing job
            var databaseId = await SaveToDatabase(operationData);

            // Act
            operationData = await ReloadFromDatabase(databaseId, false);
            await operationData.Restore();

            // Assert
            await AssertInterrupted(operationData);
        }

        [Test(Description = "Will restore an operation where the amount was reached.")]
        public async Task RestoreAmountReached()
        {
            // Arrange
            var operationData = await GetAmountReachedOperation(10, false, 10, 9);

            // Simulate shutdown and with producing job
            var databaseId = await SaveToDatabase(operationData);

            // Act
            operationData = await ReloadFromDatabase(databaseId, false);

            // Assert
            await AssertAmountReached(operationData);
        }

        [Test(Description = "Will restore an operation which is already completed.")]
        public async Task RestoreCompleted()
        {
            // Arrange
            var operationData = await GetCompletedOperation(10, false, 11, 9);

            // Simulate shutdown and with producing job
            var databaseId = await SaveToDatabase(operationData);

            // Act
            operationData = await ReloadFromDatabase(databaseId, false);
            await operationData.Restore();

            // Assert
            Assert.That(operationData.State.CanBegin, Is.False);
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.That(operationData.State.CanInterrupt, Is.False);
            Assert.That(operationData.State.CanAdvice);
        }

        /// <summary>
        /// Do some assertion operations on the given OperationData
        /// Will do a final report to complete the operation
        /// </summary>
        private async Task AssertAmountReached(IOperationData operationData)
        {
            Assert.That(operationData.State.CanBegin);
            Assert.That(operationData.State.CanPartialReport);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(operationData.State.CanInterrupt);
            Assert.That(operationData.State.CanAdvice);

            await AssertCompletableOperation(operationData);
        }

        private async Task AssertInterrupted(IOperationData operationData)
        {
            await AssertCompletableOperation(operationData);
        }

        private async Task AssertCompletableOperation(IOperationData operationData)
        {
            // Final Report should complete the operation
            var completedRaised = false;
            operationData.Completed += (_, _) => completedRaised = true;

            var report = new OperationReport(ConfirmationType.Final, 10, 0, User);
            await operationData.Report(report);

            Assert.That(completedRaised, $"The {nameof(IOperationData.Completed)} event was not raised.");
        }

        /// <summary>
        /// Runs some production on the first job of the operation
        /// Will finish the job after saving the operation to the database
        /// </summary>
        private async Task<long> SimulateProductionWithShutdown(IOperationData operationData)
        {
            var initialJob = operationData.Operation.Jobs.First();

            JobHandlerMock.Setup(d => d.Restore(It.IsAny<IEnumerable<long>>()))
                .Returns([initialJob]);

            // Simulate some running job
            initialJob.Classification = JobClassification.Running;
            initialJob.SuccessCount = 1;
            ((TestJob)initialJob).SetRunning(2);

            await operationData.JobStateChanged(new JobStateChangedEventArgs(initialJob, JobClassification.Idle, JobClassification.Running));

            // Save the operation
            var databaseId = await SaveToDatabase(operationData);

            // bring the job to the end
            initialJob.Classification = JobClassification.Completed;
            initialJob.SuccessCount = initialJob.Amount;
            ((TestJob)initialJob).SetRunning(0);

            return databaseId;
        }

        /// <summary>
        /// Saves the given operation to the database
        /// </summary>
        private async Task<long> SaveToDatabase(IOperationData operationData)
        {
            using var uow = _orderModel.Create();

            var orderEntity = await OperationStorage.SaveOrder(uow, _orderData);
            var operationEntity = await OperationStorage.SaveOperation(uow, operationData);

            operationEntity.Order = orderEntity;

            await uow.SaveChangesAsync();

            return operationEntity.Id;
        }

        /// <summary>
        /// Reloads the operation with the given id from the database
        /// </summary>
        private async Task<IOperationData> ReloadFromDatabase(long operationId, bool replaceScrap)
        {
            var operationData = GetOperationDataInstance(replaceScrap);

            using var uow = _orderModel.Create();

            var operationEntity = await uow.GetRepository<IOperationEntityRepository>()
                .GetByKeyAsync(operationId);

            await operationData.Initialize(operationEntity, _orderData);

            return operationData;
        }
    }
}

