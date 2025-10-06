// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.TestTools;
using Moq;
using NUnit.Framework;
using Moryx.TestTools.UnitTest;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class SingleJobOperationDispatcherTests
    {
        private Mock<IJobManagement> _jobManagementMock;
        private Mock<IOperationDataPool> _operationPoolMock;
        private Mock<IOperationData> _operationDataMock;
        private JobHandler _jobHandler;
        private SingleJobOperationDispatcher _dispatcher;
        private IJobManagement _jobManagement;
        private IOperationData _operationData;
        private InternalOperation _operation;

        [SetUp]
        public void SetUp()
        {
            _jobManagementMock = new Mock<IJobManagement>();
            _jobManagement = _jobManagementMock.Object;

            _operation = new InternalOperation
            {
                Recipes = new List<IProductRecipe>(1) {new DummyRecipe()}
            };

            _operationDataMock = new Mock<IOperationData>();
            _operationDataMock.SetupGet(o => o.Operation).Returns(_operation);
            _operationData = _operationDataMock.Object;

            _operationPoolMock = new Mock<IOperationDataPool>();
            _operationPoolMock.Setup(p => p.Get(_operation)).Returns(_operationData);
            _operationPoolMock.Setup(p => p.GetAll(It.IsAny<Func<IOperationData, bool>>())).Returns(new[] {_operationData});

            _dispatcher = new SingleJobOperationDispatcher
            {
                JobManagement = _jobManagementMock.Object,
                ParallelOperations = new NotSoParallelOps(),
            };

            _jobHandler = new JobHandler
            {
                JobManagement = _jobManagementMock.Object,
                OperationDataPool = _operationPoolMock.Object,
                ParallelOperations = new NotSoParallelOps(),
                Dispatcher = _dispatcher
            };

            _jobHandler.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _jobHandler.Stop();
        }

        [Test(Description = "Job updates of job management should update job on operation.")]
        public void JobUpdateUpdatesJobOnOperation()
        {
            // Arrange
            var someJob = new Job(new ProductRecipe(), 1) {Id = 1};
            _operation.Jobs = new[] {someJob};

            // Act
            var args = new JobStateChangedEventArgs(someJob, JobClassification.Idle, JobClassification.Running);
            _jobManagementMock.Raise(j => j.StateChanged += null, _jobManagement, args);

            // Assert
            Assert.DoesNotThrow(delegate
            {
                _operationDataMock.Verify(o => o.JobStateChanged(args), Times.Once);
            }, "There should be an occurred JobStateChanged event");
        }

        [Test]
        public void DispatchAddsNewJob()
        {
            // Arrange
            const int amount = 10;
            var newJob = new Job(new ProductRecipe(), amount)
            {
                Id = 2
            };

            _jobManagementMock.Setup(j => j.Add(It.IsAny<JobCreationContext>())).Returns(new[] {newJob});

            // Act
            _jobHandler.Dispatch(_operationData, new[] {new DispatchContext(new DummyRecipe(), amount)});

            // Assert
            Assert.DoesNotThrow(delegate
            {
                _operationDataMock.Verify(o => o.AddJob(newJob), Times.Once);
            }, "There should be an added OperationData");
        }

        [Test(Description = "If the operation have multiple jobs, the new dispatched job should be moved after the last job of the operation")]
        public void DispatchMovesJobAfterLastOfOperation()
        {
            var lastJob = new Job(new ProductRecipe(), 1) {Id = 3, Classification = JobClassification.Completing};
            var jobs = new[]
            {
                new Job(new ProductRecipe(), 1) {Id = 1, Classification = JobClassification.Completing},
                new Job(new ProductRecipe(), 1) {Id = 2, Classification = JobClassification.Completing},
                lastJob
            };
            _operation.Jobs = jobs;

            const int amount = 10;
            var newJob = new Job(new ProductRecipe(), amount)
            {
                Id = 4
            };

            JobCreationContext createdContext = null;
            _jobManagementMock.Setup(j => j.Add(It.IsAny<JobCreationContext>())).Returns(delegate (JobCreationContext context)
            {
                createdContext = context;
                return new[] {newJob};
            });

            // Act
            _jobHandler.Dispatch(_operationData, new[] {new DispatchContext(new DummyRecipe(), amount)});

            // Assert
            Assert.That(createdContext.Position.PositionType, Is.EqualTo(JobPositionType.AfterOther));
            Assert.That(createdContext.Position.ReferenceId, Is.EqualTo(lastJob.Id));
            Assert.DoesNotThrow(delegate
            {
                _operationDataMock.Verify(o => o.AddJob(newJob), Times.Once);
            }, "There should be a MoveAfterRequest after adding the new Job");
        }

        [Test(Description = "Complete should complete all jobs of the operation")]
        public void CompleteCompletesAllJobs()
        {
            // Arrange
            var firstJob = new Job(new ProductRecipe(), 1) {Id = 1};
            var secondJob = new Job(new ProductRecipe(), 1) {Id = 2};
            var jobs = new[] {firstJob, secondJob};
            _operation.Jobs = jobs;

            // Act
            _jobHandler.Complete(_operationData);

            // Assert
            Assert.DoesNotThrow(delegate
            {
                foreach (var job in jobs)
                {
                    _jobManagementMock.Verify(j => j.Complete(job), Times.Once);
                }
            }, "There should be a call of the Complete method at the JobManagement for every job");
        }

        [Test(Description = "Restore adds the jobs by their given id back to the operation.")]
        public void RestoreAddsJobsToOperation()
        {
            // Arrange
            var firstJob = new Job(new ProductRecipe(), 1) { Id = 1 };
            var secondJob = new Job(new ProductRecipe(), 1) { Id = 2 };
            var jobs = new[] { firstJob, secondJob };

            _jobManagementMock.Setup(j => j.Get(1)).Returns(firstJob);
            _jobManagementMock.Setup(j => j.Get(2)).Returns(secondJob);

            // Act
            var restored = _jobHandler.Restore(jobs.Select(j => j.Id));

            // Assert
            Assert.That(restored.SequenceEqual(jobs), "The job handler should restore the jobs for all given ids");
            _jobManagementMock.Verify(j => j.Get(It.IsAny<long>()), Times.Exactly(2), "The job management facade should be called once for each job");
        }
    }
}
