// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Container;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Capabilities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.ControlSystem.TestTools.Tasks;
using Moryx.Logging;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Modules;
using Moryx.Notifications;
using Moryx.TestTools.UnitTest;
using Moryx.Threading;
using Moryx.Tools;
using Moryx.Workplans;
using NUnit.Framework;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ProcessesIntegrationTests
    {
        private IContainer _container;
        private IUnitOfWorkFactory<ProcessContext> _unitOfWorkFactory;

        private Mock<IProductManagement> _productManagementMock;
        private Mock<IResourceManagement> _resourceManagementMock;
        private Mock<IJobData> _jobDataMock;
        private Mock<ICell> _mountResourceMock;
        private Mock<ICell> _assignIdentityResourceMock;
        private Mock<ICell> _unmountResourceMock;
        private Mock<INotificationAdapter> _notificationAdapterMock;

        [SetUp]
        public void FixtureSetUp()
        {
            ReflectionTool.TestMode = true;

            // Prepare InMemory ControlSystem db
            _unitOfWorkFactory = new UnitOfWorkFactory<ProcessContext>(new InMemoryDbContextManager(nameof(ProcessesIntegrationTests)));
            _jobDataMock = new Mock<IJobData>();

            using (var uow = _unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<IJobEntityRepository>();
                var jobEntity = repo.Create();
                jobEntity.RecipeId = 1;
                jobEntity.RecipeProvider = "Bob";
                jobEntity.Amount = 1;
                jobEntity.State = JobStateBase.InitialKey;
                uow.SaveChanges();

                _jobDataMock.Setup(j => j.Id).Returns(jobEntity.Id);
            }

            // Prepare ProductManagement mock
            _productManagementMock = new Mock<IProductManagement>();
            _productManagementMock.Setup(p => p.CreateInstance(It.IsAny<DummyProductType>(), It.IsAny<bool>())).Returns(new DummyProductInstance());
        }

        [SetUp]
        public void SetUp()
        {
            // Prepare mounting resource mock
            _mountResourceMock = new Mock<ICell>();
            _mountResourceMock.SetupGet(r => r.Capabilities).Returns(() => new MountCapabilities(true, false));
            _mountResourceMock.SetupGet(r => r.Id).Returns(() => 1);

            // Prepare assign identity resource mock
            _assignIdentityResourceMock = new Mock<ICell>();
            _assignIdentityResourceMock.SetupGet(r => r.Capabilities).Returns(() => new AssignIdentityCapabilities(IdentitySource.Pool, 0));
            _assignIdentityResourceMock.SetupGet(r => r.Id).Returns(() => 2);

            // Prepare unmount resource mock
            _unmountResourceMock = new Mock<ICell>();
            _unmountResourceMock.SetupGet(r => r.Capabilities).Returns(() => new MountCapabilities(false, true));
            _unmountResourceMock.SetupGet(r => r.Id).Returns(() => 3);

            // Prepare ResourceManagement mock
            _resourceManagementMock = new Mock<IResourceManagement>();
            _resourceManagementMock.Setup(rm => rm.GetResources<ICell>()).Returns(() => new[]
            {
                _mountResourceMock.Object,
                _assignIdentityResourceMock.Object,
                _unmountResourceMock.Object
            });

            _notificationAdapterMock = new Mock<INotificationAdapter>();

            _resourceManagementMock.Setup(rm => rm.GetResources<ICell>(It.Is<MountCapabilities>(c => c.CanMount && !c.CanUnmount)))
                .Returns(() => new[] { _mountResourceMock.Object });

            _resourceManagementMock.Setup(rm => rm.GetResources<ICell>(It.IsAny<AssignIdentityCapabilities>()))
                .Returns(() => new[] { _assignIdentityResourceMock.Object });

            _resourceManagementMock.Setup(rm => rm.GetResources<ICell>(It.Is<MountCapabilities>(c => !c.CanMount && c.CanUnmount)))
                .Returns(() => new[] { _unmountResourceMock.Object });

            // Prepare container
            var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
            _container = new CastleContainer();
            _container.Register<IParallelOperations, NotSoParallelOps>();
            _container.LoadFromAssembly(typeof(ProcessController).Assembly);
            _container.SetInstance(_unitOfWorkFactory);
            _container.SetInstance<IModuleLogger>(logger);
            _container.SetInstance(_productManagementMock.Object);
            _container.SetInstance(_resourceManagementMock.Object);
            _container.SetInstance(_notificationAdapterMock.Object);
            _container.SetInstance(new ModuleConfig { RemovalMessage = "Foo", ResourceSelectors = new List<CellSelectorConfig>() });
            // Load components and boot
            _container.Resolve<IProcessStorage>().Start();
            var listeners = _container.ResolveAll<IActivityPoolListener>().OrderBy(l => l.StartOrder);
            foreach (var listener in listeners)
            {
                listener.Initialize();
                listener.Start();
            }
        }

        [TearDown]
        public void Destroy()
        {
            _container.Destroy();
        }

        [TestCase(true, Description = "Execute process with product.")]
        [TestCase(false, Description = "Execute process without product.")]
        public void ExecuteWorkplan(bool withProduct)
        {
            // Arrange: Create a fake job
            if (withProduct)
            {
                _jobDataMock.Setup(j => j.Recipe).Returns(new ProductionRecipe
                {
                    Product = new DummyProductType(),
                    Workplan = CreateStraightWorkplan()
                });
            }
            else
            {
                _jobDataMock.Setup(j => j.Recipe).Returns(new WorkplanRecipe
                {
                    Workplan = CreateStraightWorkplan()
                });
            }

            // Act: Start on controller
            var controller = _container.Resolve<IProcessController>();
            ((IInitializable)controller).Initialize();
            controller.ProcessChanged += delegate { };
            var process = _jobDataMock.Object.Recipe.CreateProcess();
            var processData = new ProcessData(process)
            {
                Id = IdShiftGenerator.Generate(_jobDataMock.Object.Id, (DateTime.Now.Millisecond + (withProduct ? 1 : 0))),
                Job = _jobDataMock.Object
            };
            controller.Start(processData);

            _mountResourceMock.Setup(r => r.StartActivity(It.IsAny<ActivityStart>()))
                .Callback((ActivityStart startActivity) => CompleteActivity(_mountResourceMock, startActivity, (long)MountingResult.Mounted))
                .Verifiable("There was no MountingActivity to mount the article");

            _assignIdentityResourceMock.Setup(r => r.StartActivity(It.IsAny<ActivityStart>()))
                .Callback((ActivityStart startActivity) => CompleteActivity(_assignIdentityResourceMock, startActivity, (long)AssignIdentityResults.Assigned))
                .Verifiable("There was no AssignIdentityActivity");

            _unmountResourceMock.Setup(r => r.StartActivity(It.IsAny<ActivityStart>()))
                .Callback((ActivityStart startActivity) => CompleteActivity(_unmountResourceMock, startActivity, (long)UnmountingResult.Removed))
                .Verifiable("There was no MountingActivity to unmount the article");

            // Publish first rtws
            _mountResourceMock.Raise(r => r.ReadyToWork += null, _mountResourceMock.Object,
                Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull));

            _assignIdentityResourceMock.Raise(r => r.ReadyToWork += null, _assignIdentityResourceMock.Object,
                Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, process.Id));

            _unmountResourceMock.Raise(r => r.ReadyToWork += null, _unmountResourceMock.Object,
                Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, process.Id));

            //Verify all activity handling on resources
            _mountResourceMock.Verify();
            _assignIdentityResourceMock.Verify();
            _unmountResourceMock.Verify();

            Assert.That(ProcessState.Success, Is.EqualTo(processData.State), "The process should be successfully finished");

            _mountResourceMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Once,
                "There should one StartActivity call in the mount resource");

            _assignIdentityResourceMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Once,
                "There should one StartActivity call in the assign identity resource");

            _unmountResourceMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Once,
                "There should one StartActivity call in the unmount resource");

            _mountResourceMock.Verify(r => r.SequenceCompleted(It.IsAny<SequenceCompleted>()), Times.Once,
                "There should one SequenceCompleted call in the mount resource");

            _assignIdentityResourceMock.Verify(r => r.SequenceCompleted(It.IsAny<SequenceCompleted>()), Times.Once,
                "There should one SequenceCompleted call in the assign identity resource");

            _unmountResourceMock.Verify(r => r.SequenceCompleted(It.IsAny<SequenceCompleted>()), Times.Once,
                "There should one SequenceCompleted call in the unmount resource");
        }

        private static IWorkplan CreateStraightWorkplan()
        {
            var workplan = new Workplan();

            // Connectors
            var start = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
            var end = WorkplanInstance.CreateConnector("End", NodeClassification.End);
            var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);

            workplan.Add(start, end, failed);

            // Steps
            var mountCompleted = workplan.AddConnector("Mount completed");
            workplan.AddStep(new MountTask(), new MountingParameters(), start, mountCompleted, failed);

            var identityAssigned = workplan.AddConnector("Identity assigned");
            workplan.AddStep(new AssignIdentityTask(), new AssignIdentityParameters(), mountCompleted, identityAssigned, failed, failed, failed);

            workplan.AddStep(new UnmountTask(), new MountingParameters(), identityAssigned, end, failed, failed);

            return workplan;
        }

        private static void CompleteActivity(Mock<ICell> cellMock, ActivityStart activity, long result)
        {
            var activityResult = activity.CreateResult(result);
            cellMock.Raise(r => r.ActivityCompleted += null, cellMock.Object, activityResult);
        }
    }
}
