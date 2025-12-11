// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moq;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.TestTools;
using Moryx.AbstractionLayer.Workplans;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Identity;
using Moryx.Model;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.TestTools.UnitTest;
using Moryx.Tools;
using NUnit.Framework;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class StorageTriggerTest : ProcessTestsBase
    {
        private IActivityPoolListener _articleArchiver;
        private Mock<IProductManagement> _productManagementMock;
        private IUnitOfWorkFactory<ProcessContext> _unitOfWorkFactory;

        private ProcessStorage _storage;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            ReflectionTool.TestMode = true;

            // Prepare InMemory resource db
            _unitOfWorkFactory = new UnitOfWorkFactory<ProcessContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));

            using (var uow = _unitOfWorkFactory.Create())
            {
                var jobRepo = uow.GetRepository<IJobEntityRepository>();
                var entity = jobRepo.Create();
                entity.RecipeProvider = "Foo";
                entity.Amount = 100;
                entity.State = 1;

                uow.SaveChanges();
            }
        }

        [SetUp]
        public void Create()
        {
            CreateList();

            _productManagementMock = new Mock<IProductManagement>();
            _productManagementMock.Setup(p => p.GetInstanceAsync(It.IsAny<long>())).ReturnsAsync(() => new DummyProductInstance());

            _storage = new ProcessStorage { UnitOfWorkFactory = _unitOfWorkFactory };
            _storage.Start();

            _articleArchiver = new StorageTrigger
            {
                ActivityPool = DataPool,
                ProductManagement = _productManagementMock.Object,
                ProcessStorage = _storage,
                ParallelOperations = new NotSoParallelOps()
            };

            _articleArchiver.Initialize();
            _articleArchiver.Start();
        }

        [TearDown]
        public void DestroyArticleArchiver()
        {
            _articleArchiver.Stop();
            ((IDisposable)_articleArchiver).Dispose();
            _productManagementMock = null;
            DestroyList();
        }

        [TestCase(ProcessTypes.Process, ProcessState.Initial, ProcessState.Initial, Description = "Nothing happens")]
        [TestCase(ProcessTypes.Process, ProcessState.Restored, ProcessState.Restored, Description = "Nothing happens")]
        [TestCase(ProcessTypes.ProductionProcessWithInstance, ProcessState.Running, ProcessState.Running, Description = "Nothing happens")]
        [TestCase(ProcessTypes.ProductionProcessWithInstanceReference, ProcessState.Restored, ProcessState.RestoredReady, Description = "Should get article for restored process")]
        [TestCase(ProcessTypes.ProductionProcessWithInstanceReference, ProcessState.CleaningUp, ProcessState.CleaningUpReady, Description = "Should get article for cleaning-up process")]
        [TestCase(ProcessTypes.ProductionProcess, ProcessState.Restored, ProcessState.RestoredReady, Description = "Should create article for restored process")]
        [TestCase(ProcessTypes.ProductionProcess, ProcessState.CleaningUp, ProcessState.CleaningUpReady, Description = "Should create article for cleaning-up process")]
        [TestCase(ProcessTypes.ProductionProcess, ProcessState.Ready, ProcessState.Ready)]
        public void ProvideArticle(ProcessTypes processType, ProcessState triggerState, ProcessState nextState)
        {
            // Arrange
            var process = PrepareProcessData(processType);
            process.State = triggerState;
            _productManagementMock.Setup(p => p.GetInstanceAsync(It.IsAny<long>())).ReturnsAsync((long id) => new DummyProductInstance { Id = id });

            // Act
            DataPool.AddProcess(process);
            if (triggerState == ProcessState.Ready)
            {
                var activity = CreateActivityData(InstanceModificationType.Created, process.Id);
                DataPool.AddActivity(process, activity);

                activity.Activity.Complete(0);
                DataPool.UpdateActivity(activity, ActivityState.ResultReceived);
            }

            // Assert
            Assert.That(process.State, Is.EqualTo(nextState));
            if (processType >= ProcessTypes.ProductionProcess)
            {
                var instance = ((ProductionProcess)process.Process).ProductInstance;
                Assert.That(instance, Is.Not.Null);
                if (triggerState == ProcessState.Ready)
                    _productManagementMock.Verify(p => p.SaveInstanceAsync(It.IsAny<DummyProductInstance>()), Times.Once, "There should be a SaveInstance call");

            }
        }

        [TestCase(ProcessState.Success, 42, Description = "Test if an article is saved as successful.")]
        [TestCase(ProcessState.Failure, 27, Description = "Test if an article is saved as failure.")]
        public void SaveArticleOnProcessChange(ProcessState processState, long instanceId)
        {
            // Arrange
            var process = PrepareProcessData(ProcessTypes.ProductionProcessWithInstance, instanceId);
            DataPool.AddProcess(process);
            ((ProductionProcess)process.Process).ProductInstance.Id = instanceId;

            // Act
            DataPool.UpdateProcess(process, processState);

            // Assert
            using (var uow = _unitOfWorkFactory.Create())
            {
                var entity = uow.GetEntity<ProcessEntity>(process.Process);
                Assert.That(entity, Is.Not.Null);
                Assert.That(entity.ReferenceId, Is.EqualTo(instanceId));
                Assert.That(entity.State, Is.EqualTo((int)processState));
            }
            Assert.DoesNotThrow(delegate
            {
                _productManagementMock.Verify(p => p.SaveInstanceAsync(It.IsAny<DummyProductInstance>()), Times.Once, "There should be a SaveInstance call");
            });
        }

        [TestCase(ProcessState.Success, ProcessTypes.Process, Description = "Test if no article will be saved because it is not production process.")]
        [TestCase(ProcessState.Running, ProcessTypes.ProductionProcess, Description = "Test if no article will be saved because it has the wrong ProcessSchedulingState")]
        public void NoArticleSaveOnProcessChange(ProcessState processState, ProcessTypes processType)
        {
            // Arrange
            var process = PrepareProcessData(processType);
            DataPool.AddProcess(process);

            // Act
            DataPool.UpdateProcess(process, processState);

            // Assert
            Assert.DoesNotThrow(() => _productManagementMock.Verify(p => p.SaveInstanceAsync(It.IsAny<DummyProductInstance>()), Times.Never, "There should be no SaveArticle call"));
        }

        [TestCase(4, ProcessTypes.Process, ActivityState.ResultReceived, Description = "Test if the process is not a production process.")]
        [TestCase(8, ProcessTypes.ProductionProcessWithInstance, ActivityState.ResultReceived, Description = "Test if it will stop because nothing is to save.")]
        [TestCase(2, ProcessTypes.ProductionProcessWithInstanceUnsaved, ActivityState.Configured, Description = "Test if it will not save anything when the Activity state is not CompleteSaved.")]
        [TestCase(16, ProcessTypes.ProductionProcessWithInstanceUnsaved, ActivityState.ResultReceived, Description = "Test if an article was saved.")]
        public void SaveOnActivityChange(long articleId, ProcessTypes processType, ActivityState activityState)
        {
            // Arrange
            var process = PrepareProcessData(processType, articleId);
            var modification = processType == ProcessTypes.ProductionProcessWithInstanceUnsaved
                ? InstanceModificationType.Changed : InstanceModificationType.None;
            var activity = CreateActivityData(modification, process.Id);
            DataPool.AddProcess(process);
            if (processType > ProcessTypes.ProductionProcessWithInstance)
                ((ProductionProcess)process.Process).ProductInstance.Id = articleId;
            DataPool.AddActivity(process, activity);

            // Act
            activity.Activity.Complete(0);
            DataPool.UpdateActivity(activity, activityState);

            // Assert
            if (activityState == ActivityState.ResultReceived)
            {
                using (var uow = _unitOfWorkFactory.Create())
                {
                    var entity = uow.GetRepository<IActivityEntityRepository>().GetByKey(activity.Id);
                    Assert.That(entity, Is.Not.Null);
                    Assert.That(entity.ProcessId, Is.EqualTo(process.Id));
                }
            }
            Assert.DoesNotThrow(delegate
            {
                if (processType < ProcessTypes.ProductionProcessWithInstanceUnsaved || activityState < ActivityState.ResultReceived)
                {
                    _productManagementMock.Verify(p => p.SaveInstanceAsync(It.IsAny<ProductInstance>()), Times.Never,
                        "There should be no SaveArticle call if the Activity is not completed");
                }
                if (processType == ProcessTypes.ProductionProcessWithInstanceUnsaved && activityState == ActivityState.ResultReceived)
                {
                    _productManagementMock.Verify(p => p.SaveInstanceAsync(It.Is<ProductInstance>(a => a.Id == articleId)), Times.Once,
                        "There should be a SaveArticle call with the article id: " + articleId);
                }
            });
        }

        [Test, Description("Set Rework flag on article and process if an activity reports loading an article")]
        public void SetReworkFlagOnLoadModification()
        {
            // Arrange
            var process = PrepareProcessData(ProcessTypes.ProductionProcess);
            DataPool.AddProcess(process);

            // Act
            var activity = CreateActivityData(InstanceModificationType.Loaded, process.Id);
            DataPool.AddActivity(process, activity);

            activity.Activity.Complete(0);
            DataPool.UpdateActivity(activity, ActivityState.ResultReceived);

            // Assert
            Assert.That(process.Rework, Is.True);
            Assert.DoesNotThrow(delegate
            {
                _productManagementMock.Verify(p => p.SaveInstanceAsync(It.IsAny<ProductInstance>()), Times.Once, "There should be one SaveArticle call");
            });
        }

        [TestCase(false, Description = "Article not failed, therefore not flagged rework")]
        [TestCase(true, Description = "Article previously failed, therefore flagged rework")]
        public void LoadArticleByIdentity(bool rework)
        {
            // Arrange
            _productManagementMock.Setup(p => p.GetInstanceAsync(It.IsAny<IIdentity>()))
                .ReturnsAsync((IIdentity id) => new IdentityDummyInstance { Identity = id, State = rework ? ProductInstanceState.Failure : ProductInstanceState.InProduction });
            var process = PrepareProcessData(ProcessTypes.ProductionProcess);
            DataPool.AddProcess(process);

            // Act
            var identity = new NumberingSchemeIdentity(0, "1234");
            var activity = CreateActivityData(InstanceModificationType.Loaded, process.Id, identity);
            DataPool.AddActivity(process, activity);

            activity.Activity.Complete(0);
            DataPool.UpdateActivity(activity, ActivityState.ResultReceived);

            // Assert
            Assert.That(process.Rework, Is.EqualTo(rework));
            var article = (process.Process as ProductionProcess)?.ProductInstance as IdentityDummyInstance;
            Assert.That(article, Is.Not.Null);
            Assert.That(article.Identity, Is.EqualTo(identity));
            Assert.DoesNotThrow(delegate
            {
                _productManagementMock.Verify(p => p.GetInstanceAsync(It.IsAny<IIdentity>()), Times.Once, "There should be one GetArticle call");
            });
        }

        private class IdentityDummyInstance : DummyProductInstance, IIdentifiableObject
        {
            public IIdentity Identity { get; set; }
        }

        private ProcessData PrepareProcessData(ProcessTypes processType, long instanceId = 1)
        {
            Process process;
            var recipe = new ProductRecipe
            {
                Product = new DummyProductType
                {
                    Id = 2,
                    Identity = new ProductIdentity("12345678", 1)
                }
            };
            switch (processType)
            {
                case ProcessTypes.Process:
                    process = new Process();
                    break;
                case ProcessTypes.ProductionProcess:
                    process = new ProductionProcess { Recipe = recipe };
                    break;
                case ProcessTypes.ProductionProcessWithInstance:
                    process = new ProductionProcess
                    {
                        Recipe = recipe,
                        ProductInstance = new DummyProductInstance { Id = instanceId }
                    };
                    break;
                case ProcessTypes.ProductionProcessWithInstanceUnsaved:
                    process = new ProductionProcess
                    {
                        Recipe = recipe,
                        ProductInstance = new DummyProductInstance { Id = instanceId }
                    };
                    break;
                case ProcessTypes.ProductionProcessWithInstanceReference:
                    process = new ProductionProcess
                    {
                        Recipe = recipe
                    };
                    break;
                default:
                    process = null;
                    break;
            }

            var jobDataMock = new Mock<IJobData>();
            jobDataMock.SetupGet(j => j.Id).Returns(1);

            return new ProcessData(process)
            {
                Id = IdShiftGenerator.Generate(jobDataMock.Object.Id << IdShiftGenerator.ShiftSpace, NextId),
                Job = jobDataMock.Object,
                State = ProcessState.Ready,
                ReferenceId = instanceId
            };
        }

        private ActivityData CreateActivityData(InstanceModificationType modification, long processId, IIdentity instanceIdentity = null)
        {
            var activity = new DummyActivity
            {
                InstanceIdentity = instanceIdentity,
                ModificationType = modification,
            };
            var activityData = new ActivityData(activity)
            {
                Id = IdShiftGenerator.Generate(processId << IdShiftGenerator.ShiftSpace, NextId),
                Resource = new CellReference(1),
                Task = new TaskTransition<DummyActivity>(new DummyActivityParameters(), null)
            };

            return activityData;
        }

        /// <summary>
        /// enum to prepare a process.
        /// </summary>
        public enum ProcessTypes
        {
            Process = 1,
            ProductionProcess = 2,
            ProductionProcessWithInstance = 3,
            ProductionProcessWithInstanceUnsaved = 4,
            ProductionProcessWithInstanceReference = 5,
        }
    }
}

