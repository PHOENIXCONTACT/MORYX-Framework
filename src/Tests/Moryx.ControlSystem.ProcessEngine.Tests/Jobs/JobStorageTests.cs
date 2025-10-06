// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.TestTools.UnitTest;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class JobStorageTests
    {
        private IUnitOfWorkFactory<ProcessContext> _unitOfWorkFactory;
        private JobStorage _jobStorage;
        private IJobHistory _jobHistory;
        private IRecipeProvider _recipeProvider;
        private NotSoParallelOps _notSoParallelOps;

        [SetUp]
        public void TestFixtureSetUp()
        {
            _unitOfWorkFactory = BuildUnitOfWorkFactory();

            var recipeProviderMock = new Mock<IRecipeProvider>();
            var productMock = recipeProviderMock.As<IProductManagement>(); // Is has to be a RecipeProvider and also a ProductManagement
            recipeProviderMock.SetupGet(r => r.Name).Returns("FooProvider");
            recipeProviderMock.Setup(m => m.LoadRecipe(It.IsAny<long>())).Returns(new Mock<IProductionRecipe>().Object);

            _recipeProvider = recipeProviderMock.Object;

            var internalJobFactory = new Mock<IContainerJobDataFactory>();
            internalJobFactory.Setup(f => f.CreateProductionJob(It.IsAny<IWorkplanRecipe>(), It.IsAny<JobEntity>()))
                            .Returns<IWorkplanRecipe, JobEntity>((recipe, entity) =>
                            {
                                var mock = new Mock<IProductionJobData>();
                                mock.SetupGet(m => m.Amount).Returns(entity.Amount);
                                mock.SetupGet(m => m.Id).Returns(entity.Id);
                                mock.SetupGet(m => m.State).Returns(new Mock<IJobState>().Object);
                                return mock.Object;
                            });

            _notSoParallelOps = new();

            _jobStorage = new JobStorage
            {
                ProductManagement = productMock.Object,
                UnitOfWorkFactory = _unitOfWorkFactory,
                ParallelOperations = _notSoParallelOps,
                JobFactory = new JobDataFactory { InternalFactory = internalJobFactory.Object }
            };

            _jobHistory = _jobStorage;
        }

        protected virtual UnitOfWorkFactory<ProcessContext> BuildUnitOfWorkFactory()
        {
            return new UnitOfWorkFactory<ProcessContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetCompleted()
        {
            // Arrange
            const long jobId = 1;
            FillDatabase();

            // Act
            var job = _jobHistory.Get(jobId);

            // Assert
            Assert.That(job, Is.Not.Null, "This job should be found by the JobHistory");
            Assert.That(job.Classification, Is.EqualTo(JobClassification.Completed));
        }

        [Test]
        public void GetUncompleted()
        {
            // Arrange
            const long jobId = 2;
            FillDatabase();

            // Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                // Act
                _jobHistory.Get(jobId);
            });
        }

        [Test]
        public void AddJobToJobStorageEndAndCheckOrder()
        {
            // Arrange
            FillDatabase();
            var jobsToAdd = CreateJobDataMocks(10);

            // Act
            _jobStorage.Save(new ModifiedJobsFragment(jobsToAdd, 2));

            // Assert
            var jobs = _jobStorage.GetAll().ToArray();

            Assert.That(jobs.Length, Is.EqualTo(11));
            Assert.That(jobs[0].Amount, Is.EqualTo(21));

            for (int idx = 1; idx < jobs.Length; idx++)
            {
                Assert.That(jobs[idx].Amount, Is.EqualTo(idx - 1));
            }
        }

        [Test]
        public void InsertJobToJobStorageEndAndCheckOrder()
        {
            // Arrange
            FillDatabase();
            var currentJob = _jobStorage.GetAll()[0];
            var jobList = CreateJobDataMocks(10);
            jobList.Add(currentJob);

            // Act
            _jobStorage.Save(new ModifiedJobsFragment(jobList, null));

            // Assert
            var jobs = _jobStorage.GetAll().ToArray();

            Assert.That(jobs.Length, Is.EqualTo(11));
            Assert.That(jobs.Last().Amount, Is.EqualTo(21));

            for (int idx = 0; idx < jobs.Length - 1; idx++)
            {
                Assert.That(jobs[idx].Amount, Is.EqualTo(idx));
            }
        }

        private void FillDatabase()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                var jobRepo = uow.GetRepository<IJobEntityRepository>();

                // Add a completed job
                var firstEntity = jobRepo.Create();
                firstEntity.RecipeId = 1;
                firstEntity.RecipeProvider = _recipeProvider.Name;
                firstEntity.Amount = 20;
                firstEntity.State = JobStateBase.CompletedKey;

                // Add a job which have an other state key
                var secondEntity = jobRepo.Create();
                secondEntity.RecipeId = 14;
                secondEntity.RecipeProvider = _recipeProvider.Name;
                secondEntity.Amount = 21;
                secondEntity.State = int.MaxValue;

                uow.SaveChanges();
            }
        }

        private List<IJobData> CreateJobDataMocks(int amount)
        {
            var jobs = new List<IJobData>();

            for (var i = 0; i < amount; ++i)
            {
                var mock = new Mock<IJobData>();
                var managedMock = mock.As<IPersistentObject>();
                managedMock.SetupProperty(m => m.Id);

                mock.SetupGet(m => m.Id).Returns(() => managedMock.Object.Id);
                mock.SetupGet(m => m.Amount).Returns(i);
                mock.SetupGet(m => m.RecipeProvider).Returns(_recipeProvider);
                mock.SetupGet(m => m.Recipe).Returns(new Mock<IProductionRecipe>().Object);
                mock.SetupGet(m => m.State).Returns(new Mock<IJobState>().Object);

                jobs.Add(mock.Object);
            }

            return jobs;
        }
    }
}
