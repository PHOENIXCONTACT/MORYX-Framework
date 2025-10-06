// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.ControlSystem.TestTools.Tasks;
using Moryx.Logging;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Notifications;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Workplans;
using Newtonsoft.Json;
using NUnit.Framework;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ActivityProviderTests : ProcessTestsBase
    {
        private ActivityProvider _provider;
        private IUnitOfWorkFactory<ProcessContext> _unitOfWorkFactory;
        private readonly IModuleLogger _logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
        private ProductionJobData _job;
        private ProcessStorage _storage;

        private const string TokenJson = "[{\"$type\":\"Moryx.Workplans.MainToken, Moryx\",\"Name\":\"MainToken\"}]";

        [SetUp]
        public void GlobalSetup()
        {
            ReflectionTool.TestMode = true;

            // Prepare in memory ControlSystem db
            _unitOfWorkFactory = new UnitOfWorkFactory<ProcessContext>(new InMemoryDbContextManager(nameof(ProcessesIntegrationTests)));

            _job = new ProductionJobData(new DummyRecipe(), 100);

            using (var uow = _unitOfWorkFactory.Create())
            {
                var jobRepo = uow.GetRepository<IJobEntityRepository>();
                var jobEntity = jobRepo.Create();
                jobEntity.RecipeId = 1;
                jobEntity.RecipeProvider = "";
                jobEntity.Amount = _job.Amount;
                jobEntity.State = JobStateBase.InitialKey;

                uow.SaveChanges();

                ((IPersistentObject)_job).Id = jobEntity.Id;
            }
        }

        [SetUp]
        public void BuildActivityProvider()
        {
            CreateList();

            _storage = new ProcessStorage
            {
                UnitOfWorkFactory = _unitOfWorkFactory
            };
            _storage.Start();

            var notificationMock = new Mock<INotificationAdapter>();

            _provider = new ActivityProvider
            {
                Logger = _logger,
                ActivityPool = DataPool,
                UnitOfWorkFactory = _unitOfWorkFactory,
                ProcessStorage = _storage,
                NotificationAdapter = notificationMock.Object
            };
            _provider.Initialize();
            _provider.Start();
        }

        [TearDown]
        public void DestroyProvider()
        {
            _provider.Dispose();
            DestroyList();
        }

        private ProcessData CreateProcess()
        {
            var recipe = DummyRecipe.BuildRecipe();
            var process = new ProcessData(new Process { Recipe = recipe })
            {
                Job = _job,
                State = ProcessState.Ready,
            };

            return process;
        }

        [Test]
        public void ProvideActivity()
        {
            // Arrange
            var process = CreateProcess();

            // Act
            DataPool.AddProcess(process);

            // Assert
            Assert.That(ModifiedActivity, Is.Not.Null);
            Assert.That(ModifiedActivity.Activity, Is.InstanceOf<MountActivity>());
        }

        [Test]
        public void ProvideNextActivity()
        {
            // Arrange
            var process = CreateProcess();
            DataPool.AddProcess(process);

            // Act
            ModifiedActivity.Activity.Complete(0);
            var oldActivity = ModifiedActivity;
            DataPool.UpdateActivity(ModifiedActivity, ActivityState.ResultProcessed);

            // Assert: Next activity in pool
            Assert.That(oldActivity.State, Is.EqualTo(ActivityState.EngineProceeded));
            Assert.That(process.Activities.Last().Activity, Is.InstanceOf<AssignIdentityActivity>());
        }

        [Test]
        public void TolerateActivityCreationException()
        {
            // Arrange
            var process = CreateProcess();
            var assignStep = ((IWorkplanRecipe)process.Recipe).Workplan.Steps.ElementAt(1);
            ((AssignIdentityTask)assignStep).Parameters = new FaultyParameters();
            DataPool.AddProcess(process);

            // Act
            ModifiedActivity.Activity.Complete(0);
            DataPool.UpdateActivity(ModifiedActivity, ActivityState.ResultProcessed);

            // Assert: Next activity in pool
            Assert.That(ModifiedActivity.State, Is.EqualTo(ActivityState.EngineProceeded));
        }

        private class FaultyParameters : AssignIdentityParameters
        {
            protected override void Populate(IProcess process, Parameters instance)
            {
                throw new InvalidOperationException();
            }
        }

        [TestCase(true, ProcessState.Success, Description = "Complete workflow on good path")]
        [TestCase(false, ProcessState.Failure, Description = "Complete workflow on failed path")]
        public void CompleteProcess(bool success, ProcessState expectedResult)
        {
            // Arrange
            var process = CreateProcess();
            DataPool.AddProcess(process);

            // Act: Complete all activities
            var activity = process.Activities.Last();
            do
            {
                if (success)
                    activity.Activity.Complete(0);
                else
                    activity.Activity.Fail();

                DataPool.UpdateActivity(activity, ActivityState.ResultProcessed); // This call replaces the reference in ModifedActivity
            } while ((activity = process.Activities.Last()).State < ActivityState.EngineProceeded); // ModifedActivity is not replaced when last task was completed

            // Assert
            Assert.That(process.State, Is.EqualTo(expectedResult));
        }

        [Test]
        public void SaveWorkflow()
        {
            // Arrange
            var processData = CreateProcess();
            DataPool.AddProcess(processData);
            using (var uow = _unitOfWorkFactory.Create())
            {
                processData.Id = IdShiftGenerator.Generate(1, NextId);
                ModifiedActivity.Id = IdShiftGenerator.Generate(processData.Id, NextId);
                _storage.SaveProcess(processData);

                uow.SaveChanges();
            }
            DataPool.UpdateActivity(ModifiedActivity, ActivityState.Configured);

            // Act
            DataPool.UpdateProcess(processData, ProcessState.Interrupted);

            // Assert
            using (var uow = _unitOfWorkFactory.Create())
            {
                var processRepo = uow.GetRepository<IProcessEntityRepository>();
                var processEntity = processRepo.GetByKey(processData.Id);
                var tokenHolders = processEntity.TokenHolders;

                Assert.That(tokenHolders.Count, Is.EqualTo(1));
                var holder = tokenHolders.First();
                var tokens = JsonConvert.DeserializeObject<IToken[]>(holder.Tokens, JsonSettings.Minimal);
                Assert.That(tokens.Length, Is.EqualTo(1));
                var mainToken = holder.Tokens;
                Assert.That(mainToken, Is.EqualTo(TokenJson));
            }
        }

        [Test]
        public void LoadWorkflow()
        {
            // Arrange
            var processData = CreateProcess();
            using (var uow = _unitOfWorkFactory.Create())
            {
                CreateProcessInDb(_storage, uow, processData, TokenJson);
            }

            // Act
            processData = new ProcessData(processData.Process)
            {
                State = ProcessState.Initial
            };
            DataPool.AddProcess(processData);
            DataPool.UpdateProcess(processData, ProcessState.RestoredReady);
            var activities = processData.Activities;

            // Assert
            Assert.That(activities, Is.Not.Null);
            Assert.That(activities.Count, Is.EqualTo(2));
            var first = activities.First();
            Assert.That(first.State, Is.EqualTo(ActivityState.Completed));
            var second = activities.Last();
            Assert.That(second.State, Is.EqualTo(ActivityState.Initial));
            Assert.That(second.Activity, Is.InstanceOf<MountActivity>());
        }

        [Test]
        public void ResumeAndDelete()
        {
            // Arrange
            var processData = CreateProcess();
            using (var uow = _unitOfWorkFactory.Create())
            {
                CreateProcessInDb(_storage, uow, processData, TokenJson);
            }

            // Act
            processData.State = ProcessState.Initial;
            DataPool.AddProcess(processData);
            DataPool.UpdateProcess(processData, ProcessState.RestoredReady);

            // Execute workplan till completion
            var currentActivity = ModifiedActivity;
            do
            {
                currentActivity.Activity.Complete(0);
                DataPool.UpdateActivity(currentActivity, ActivityState.ResultProcessed);
                currentActivity = processData.Activities.Last();
            } while (currentActivity.State < ActivityState.EngineProceeded);

            // Make sure all tokenholders and tokens were removed
            Assert.That(processData.State, Is.EqualTo(ProcessState.Success));
            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<IProcessEntityRepository>();
                var processEntity = repo.GetByKey(processData.Id);
                Assert.That(processEntity.TokenHolders.Count, Is.EqualTo(0));
            }
        }
    }
}

