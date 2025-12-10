// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.TestTools;
using Moryx.Logging;
using Moq;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Notifications;
using Moryx.Orders.Management.Assignment;
using Moryx.Users;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.Orders.Management.Tests
{
    /// <summary>
    /// Base class for operation data tests
    /// </summary>
    public abstract class OperationDataTestBase
    {
        protected IModuleLogger Logger { get; private set; }

        internal Mock<IJobHandler> JobHandlerMock { get; private set; }

        internal Mock<IOperationAssignment> AssignmentMock { get; private set; }

        internal Mock<INotificationAdapter> NotificationAdapterMock { get; private set; }

        protected User User { get; private set; }

        private int _jobIdCounter;

        [SetUp]
        public virtual void SetUp()
        {
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
            JobHandlerMock = new Mock<IJobHandler>();
            AssignmentMock = new Mock<IOperationAssignment>();
            NotificationAdapterMock = new Mock<INotificationAdapter>();

            var userMock = new Mock<User>();
            userMock.SetupGet(u => u.Identifier).Returns("1234");
            User = userMock.Object;

            var dispatchDelegate = new Action<IOperationData, IReadOnlyList<DispatchContext>>(delegate (IOperationData operationData, IReadOnlyList<DispatchContext> dispatchContexts)
            {
                var recipe = operationData.Operation.Recipes.First();

                foreach (var dispatchContext in dispatchContexts)
                {
                    operationData.AddJob(new TestJob(recipe, (int)dispatchContext.Amount)
                    {
                        Id = ++_jobIdCounter,
                        Classification = JobClassification.Waiting,
                    });
                }
            });

            JobHandlerMock.Setup(d => d.Dispatch(It.IsAny<IOperationData>(), It.IsAny<IReadOnlyList<DispatchContext>>())).Callback(dispatchDelegate);
        }

        internal class TestJob : Job, IPredictiveJob
        {
            public TestJob(IRecipe recipe, int amount) : base(recipe, amount)
            {
                RunningProcesses = new List<IProcess>();
                AllProcesses = new List<IProcess>();
            }

            public TestJob SetRunning(int amount)
            {
                RunningProcesses = Enumerable.Range(1, amount).Select(i => new Process { Id = i }).ToList();
                return this;
            }

            public IReadOnlyList<IProcess> PredictedFailures { get; set; } = new List<IProcess>();
        }

        internal async Task<IOperationData> InitializeOperationData(int amount, bool replaceScrap, int overDeliveryAmount, int underDeliveryAmount)
        {
            var orderData = new OrderData();
            orderData.Order.Number = "30022215533";

            var operationContext = new OperationCreationContext
            {
                TotalAmount = amount,
                Name = "Test Operation",
                OverDeliveryAmount = overDeliveryAmount,
                UnderDeliveryAmount = underDeliveryAmount,
                Number = "0010",
                ProductIdentifier = "123456",
                ProductRevision = 1,
                Parts = Array.Empty<PartCreationContext>(),
                PlannedStart = DateTime.Now,
                PlannedEnd = DateTime.UtcNow,
            };

            var operationData = GetOperationDataInstance(replaceScrap);
            await operationData.Initialize(operationContext, orderData, new NullOperationSource());
            operationData.Operation.Recipes.Add(new DummyRecipe { Id = 1 });

            return operationData;
        }

        internal IOperationData GetOperationDataInstance(bool replaceScrap)
        {
            var orderdata = new OrderData();
            orderdata.Initialize(new OrderCreationContext()
            {
                Number = "300000042"
            });

            return new OperationData(new NullOperationSavingContext())
            {
                Logger = Logger,
                OperationAssignment = AssignmentMock.Object,
                JobHandler = JobHandlerMock.Object,
                CountStrategy = replaceScrap ? new ReplaceScrapStrategy() : new DoNotReplaceScrapStrategy(),
                NotificationAdapter = NotificationAdapterMock.Object,
                ModuleConfig = new ModuleConfig()
                //Order and Operation must be set automatically during OperationData.Initialize!
                //Removed for OperationDataRestoreTests.RestoreAmountReached()
                //OrderData = orderdata,
                //Operation = { Number = "0010" }
            };
        }

        internal async Task<IOperationData> GetReadyOperation(int amount, bool replaceScrap, int overDeliveryAmount, int underDeliveryAmount)
        {
            var operationData = await InitializeOperationData(amount, replaceScrap, overDeliveryAmount, underDeliveryAmount);
            await operationData.Assign();
            await operationData.AssignCompleted(true);

            return operationData;
        }

        internal async Task<IOperationData> GetRunningOperation(int amount, bool replaceScrap, int overDeliveryAmount, int underDeliveryAmount)
        {
            var operationData = await GetReadyOperation(amount, replaceScrap, overDeliveryAmount, underDeliveryAmount);
            await operationData.Adjust(10, User);

            return operationData;
        }

        internal async Task<IOperationData> GetInterruptingOperation(int amount, bool replaceScrap, int overDeliveryAmount, int underDeliveryAmount)
        {
            var operationData = await GetRunningOperation(amount, replaceScrap, overDeliveryAmount, underDeliveryAmount);

            await operationData.Interrupt(User);

            return operationData;
        }

        internal async Task<IOperationData> GetAmountReachedOperation(int amount, bool replaceScrap, int overDeliveryAmount, int underDeliveryAmount)
        {
            var operationData = await GetRunningOperation(amount, replaceScrap, overDeliveryAmount, underDeliveryAmount);

            // Reach amount
            var job = operationData.Operation.Jobs.First();
            job.Classification = JobClassification.Completed;
            job.SuccessCount = 10;
            await operationData.JobStateChanged(new JobStateChangedEventArgs(job, JobClassification.Completing, JobClassification.Completed));

            return operationData;
        }

        internal async Task<IOperationData> GetInterruptedOperation(int amount, bool replaceScrap, int overDeliveryAmount, int underDeliveryAmount)
        {
            var operationData = await GetAmountReachedOperation(amount, replaceScrap, overDeliveryAmount, underDeliveryAmount);

            // Interrupt
            await operationData.Interrupt(User);

            return operationData;
        }

        internal async Task<IOperationData> GetCompletedOperation(int amount, bool replaceScrap, int overDeliveryAmount, int underDeliveryAmount)
        {
            var operationData = await GetAmountReachedOperation(amount, replaceScrap, overDeliveryAmount, underDeliveryAmount);

            // Final report
            var report = new OperationReport(ConfirmationType.Final, 5, 5, User);
            await operationData.Report(report);

            return operationData;
        }
    }
}

