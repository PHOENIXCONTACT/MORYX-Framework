// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Capabilities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.Logging;
using Moryx.Notifications;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ResourceAssignmentTests : ProcessTestsBase
    {
        private Mock<IResourceManagement> _resourceManagementMock;
        private Mock<ICell> _productionCellMock;
        private Mock<ICell> _mountCellMock;
        private Mock<ICellSelector> _selectorMock;
        private ResourceAssignment _resourceAssignment;
        private Mock<INotificationAdapter> _notificationAdapterMock;

        [SetUp]
        public void CreateResourceAssignment()
        {
            CreateList();

            _resourceManagementMock = new Mock<IResourceManagement>();
            _notificationAdapterMock = new Mock<INotificationAdapter>();

            _productionCellMock = CreateProductionCell(_resourceManagementMock);

            _mountCellMock = CreateMountCell(_resourceManagementMock, true, false);

            // Create our multi purpose selector that does nothing per default
            _selectorMock = new Mock<ICellSelector>();
            _selectorMock
                .Setup(s => s.SelectCells(It.IsAny<IActivity>(), It.IsAny<IReadOnlyList<ICell>>()))
                .Returns<IActivity, IReadOnlyList<ICell>>((ad, cells) => cells);
            var config = new CellSelectorConfig();
            var factoryMock = new Mock<ICellSelectorFactory>();
            factoryMock.Setup(f => f.Create(It.IsAny<CellSelectorConfig>()))
                .Returns<CellSelectorConfig>(c => _selectorMock.Object);

            var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
            _resourceAssignment = new ResourceAssignment()
            {
                ActivityPool = DataPool,
                ResourceManagement = _resourceManagementMock.Object,
                Logger = logger,
                NotificationAdapter = _notificationAdapterMock.Object,
                ModuleConfig = new ModuleConfig
                {
                    ResourceSelectors = new List<CellSelectorConfig> { config }
                },
                SelectorFactory = factoryMock.Object
            };
            _resourceAssignment.Initialize();
            _resourceAssignment.Start();
        }

        [TearDown]
        public void DestoryResourceAssignment()
        {
            _resourceAssignment.Stop();
            ((IDisposable)_resourceAssignment).Dispose();
            _resourceManagementMock = null;
            DestroyList();
        }

        [Test(Description = "Set cell object on completed activities")]
        public void RestoreCellOnActivity()
        {
            // Arrange
            var processData = new ProcessData(new Process());
            var activityData = new ActivityData(new MountActivity())
            {
                Resource = new CellReference(MountCellId),
                State = ActivityState.Completed
            };
            DataPool.AddProcess(processData);
            DataPool.AddActivity(processData, activityData);

            // Act
            DataPool.UpdateProcess(processData, ProcessState.EngineStarted);

            // Assert
            Assert.That(activityData.Resource, Is.Not.Null);
            Assert.That(activityData.Resource, Is.Not.InstanceOf<CellReference>());
            Assert.That(activityData.Resource, Is.EqualTo(_mountCellMock.Object), "Incorrect cell set");
        }

        [TestCase(typeof(MountActivity), MountCellId, Description = "Assign mount activity to MountingCell")]
        public void AssignResource(Type activityType, long expectedResource)
        {
            // Arrange
            var process = new ProcessData(new Process());
            var activity = (Activity)Activator.CreateInstance(activityType);
            var activityData = new ActivityData(activity);
            DataPool.AddProcess(process);

            // Act
            DataPool.AddActivity(process, activityData);

            // Assert
            Assert.That(ModifiedActivity, Is.Not.Null);
            Assert.That(ModifiedActivity, Is.EqualTo(activityData));
            Assert.That(ModifiedActivity.Targets.Any(c => c.Id == expectedResource));
            Assert.That(ModifiedActivity.State, Is.EqualTo(ActivityState.Configured));
        }

        [TestCase(false, false, Description = "Capabilities changed, but still do not match")]
        [TestCase(false, true, Description = "Capabilities changed, and now match the required ones")]
        [TestCase(true, false, Description = "Capabilities changed, and no longer match the required ones")]
        [TestCase(true, true, Description = "Capabilities changed, and still match the required ones")]
        public void CapabilitiesChanged(bool providedBefore, bool providedAfter)
        {
            // Arrange
            var process = new ProcessData(new Process());
            var activityData = new ActivityData(new MountActivity());
            DataPool.AddProcess(process);

            Notification someNotification = null;
            _notificationAdapterMock
                .Setup(na => na.Publish(It.IsAny<INotificationSender>(), It.IsAny<Notification>()))
                .Callback<INotificationSender, Notification>((sender, notification) => someNotification = notification);
            _notificationAdapterMock
                .Setup(na => na.GetPublished(It.IsAny<INotificationSender>(), activityData))
                .Returns([someNotification]);

            // Act
            if (providedBefore)
            {
                _resourceManagementMock
                    .Setup(rm => rm.GetResources<ICell>(It.IsAny<MountCapabilities>()))
                    .Returns(() => [_mountCellMock.Object]);
            }
            else
            {
                _resourceManagementMock
                    .Setup(rm => rm.GetResources<ICell>(It.IsAny<MountCapabilities>()))
                    .Returns(() => new List<ICell>());
            }

            // Add activity and assign
            DataPool.AddActivity(process, activityData);
            DataPool.UpdateActivity(activityData, ActivityState.Configured);

            // Check if activity was assigned
            if (providedBefore)
            {
                Assert.That(activityData.Targets.Count, Is.EqualTo(1));
                Assert.That(activityData.Targets[0], Is.EqualTo(_mountCellMock.Object));
            }
            else
            {
                Assert.That(activityData.Targets.Count, Is.EqualTo(0));
                Assert.DoesNotThrow(delegate
                    {
                        _notificationAdapterMock.Verify(na => na.Publish(It.IsAny<INotificationSender>(), It.IsAny<Notification>(), activityData), Times.Once);
                    });
            }

            // UpdateList capabilities
            if (providedAfter)
            {
                _resourceManagementMock
                    .Setup(rm => rm.GetResources<ICell>(It.IsAny<MountCapabilities>()))
                    .Returns(() => [_mountCellMock.Object]);
            }
            else
            {
                _resourceManagementMock
                    .Setup(rm => rm.GetResources<ICell>(It.IsAny<MountCapabilities>()))
                    .Returns(() => new List<ICell>());
            }
            _resourceManagementMock.Raise(rm => rm.CapabilitiesChanged += null, _mountCellMock.Object, new MountCapabilities(providedAfter, false));

            // Check if the activity was updated
            if (providedAfter)
            {
                Assert.That(activityData.Targets.Count, Is.EqualTo(1));
                Assert.That(activityData.Targets[0], Is.EqualTo(_mountCellMock.Object));
                if (!providedBefore)
                    Assert.DoesNotThrow(delegate
                    {
                        _notificationAdapterMock.Verify(na => na.AcknowledgeAll(It.IsAny<INotificationSender>(), activityData), Times.Once);
                    });
            }
            else
            {
                Assert.That(activityData.Targets.Count, Is.EqualTo(0));
                Assert.DoesNotThrow(delegate
                {
                    _notificationAdapterMock.Verify(na => na.Publish(It.IsAny<INotificationSender>(), It.IsAny<Notification>(), activityData), Times.Once);
                });
            }
        }

        [Test(Description = "Assignement of activities is updated after removal of a resource")]
        public void ResourceRemoved()
        {
            // Arrange
            var process = new ProcessData(new Process());
            var activityData = new ActivityData(new MountActivity());
            DataPool.AddProcess(process);

            Notification someNotification = null;
            _notificationAdapterMock
                .Setup(na => na.Publish(It.IsAny<INotificationSender>(), It.IsAny<Notification>()))
                .Callback<INotificationSender, Notification>((sender, notification) => someNotification = notification);
            _notificationAdapterMock
                .Setup(na => na.GetPublished(It.IsAny<INotificationSender>(), activityData))
                .Returns([someNotification]);

            // Act
            _resourceManagementMock
                .Setup(rm => rm.GetResources<ICell>(It.IsAny<MountCapabilities>()))
                .Returns(() => [_mountCellMock.Object]);

            // Add activity and assign
            DataPool.AddActivity(process, activityData);
            DataPool.UpdateActivity(activityData, ActivityState.Configured);

            // Check if activity was assigned
            Assert.That(activityData.Targets.Count, Is.EqualTo(1));
            Assert.That(activityData.Targets[0], Is.EqualTo(_mountCellMock.Object));

            // Remove resource
            _resourceManagementMock
                .Setup(rm => rm.GetResources<ICell>(It.IsAny<MountCapabilities>()))
                .Returns(() => new List<ICell>());

            _resourceManagementMock.Raise(rm => rm.ResourceRemoved += null, _resourceManagementMock.Object, _mountCellMock.Object);

            // Check if the activity was updated
            Assert.That(activityData.Targets.Count, Is.EqualTo(0));
            Assert.DoesNotThrow(delegate
            {
                _notificationAdapterMock.Verify(na => na.Publish(It.IsAny<INotificationSender>(), It.IsAny<Notification>(), activityData), Times.Once);
            });
        }

        [Test]
        public void FailActivityMissingResource()
        {
            // Arrange
            var process = new ProcessData(new Process());
            var activity = new ActivityData(new AssignIdentityActivity
            {
                Parameters = new AssignIdentityParameters
                {
                    Type = 0
                }
            });
            DataPool.AddProcess(process);

            // Act
            DataPool.AddActivity(process, activity);

            // Assert
            Assert.That(ModifiedActivity, Is.Not.Null);
            Assert.That(ModifiedActivity, Is.EqualTo(activity));
            Assert.That(ModifiedActivity.State, Is.EqualTo(ActivityState.Initial));
        }

        [Test(Description = "Resource selector reverses the order of possible resources")]
        public void SimpleSelectorTest()
        {
            // Arrange
            _selectorMock
                .Setup(s => s.SelectCells(It.IsAny<IActivity>(), It.IsAny<IReadOnlyList<ICell>>()))
                .Returns<IActivity, IReadOnlyList<ICell>>((ad, cells) => cells.Reverse().ToList());
            _resourceManagementMock.Setup(s => s.GetResources<ICell>(It.IsAny<ICapabilities>()))
                .Returns(new List<ICell> { _productionCellMock.Object, _mountCellMock.Object });

            // Act
            var process = new ProcessData(new Process());
            var activityData = new ActivityData(new MountActivity());
            DataPool.AddProcess(process);
            DataPool.AddActivity(process, activityData);

            // Assert
            Assert.That(ModifiedActivity.Targets.Count, Is.EqualTo(2));
            Assert.That(ModifiedActivity.Targets[0], Is.EqualTo(_mountCellMock.Object));
            Assert.That(ModifiedActivity.Targets[1], Is.EqualTo(_productionCellMock.Object));
        }

        [Test(Description = "Resource selector removes a resource from possible resources")]
        public void RemovalSelectorTest()
        {
            // Arrange
            _selectorMock
                .Setup(s => s.SelectCells(It.IsAny<IActivity>(), It.IsAny<IReadOnlyList<ICell>>()))
                .Returns<IActivity, IReadOnlyList<ICell>>((ad, cells) => cells.Take(1).ToList());
            _resourceManagementMock.Setup(s => s.GetResources<ICell>(It.IsAny<ICapabilities>()))
                .Returns(new List<ICell> { _productionCellMock.Object, _mountCellMock.Object });

            // Act
            var process = new ProcessData(new Process());
            var activityData = new ActivityData(new MountActivity());
            DataPool.AddProcess(process);
            DataPool.AddActivity(process, activityData);

            // Assert
            Assert.That(ModifiedActivity.Targets.Count, Is.EqualTo(1));
            Assert.That(ModifiedActivity.Targets[0], Is.EqualTo(_productionCellMock.Object));
        }

        [Test(Description = "Resource selector illegally adds a resource to the targets. Its changes are ignored!")]
        public void InvalidSelectorTest()
        {
            // Arrange
            _selectorMock
                .Setup(s => s.SelectCells(It.IsAny<IActivity>(), It.IsAny<IReadOnlyList<ICell>>()))
                .Returns<IActivity, IReadOnlyList<ICell>>((ad, cells) => cells.Concat([_mountCellMock.Object]).ToList());
            _resourceManagementMock.Setup(s => s.GetResources<ICell>(It.IsAny<ICapabilities>()))
                .Returns(new List<ICell> { _productionCellMock.Object });

            // Act
            var process = new ProcessData(new Process());
            var activityData = new ActivityData(new MountActivity());
            DataPool.AddProcess(process);
            DataPool.AddActivity(process, activityData);

            // Assert
            Assert.That(ModifiedActivity.Targets.Count, Is.EqualTo(1));
            Assert.That(ModifiedActivity.Targets[0], Is.EqualTo(_productionCellMock.Object));
        }
    }
}
