// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Logging;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Notifications;
using Moryx.Resources.Management.Model;
using NUnit.Framework;

namespace Moryx.Resources.Management.Tests
{
    [TestFixture]
    public class ResourceManagerTests
    {
        private IUnitOfWorkFactory<ResourcesContext> _modelFactory;
        private Mock<IResourceTypeController> _typeControllerMock;
        private Mock<IResourceLinker> _linkerMock;
        private ResourceMock _resourceMock;

        private ResourceGraph _graph;
        private ResourceManager _resourceManager;
        private Mock<IResourceInitializer> _initializerMock;
        private ModuleConfig _moduleConfig;

        private const string DatabaseResourceName = "Resource Mock";

        [SetUp]
        public void Setup()
        {
            _resourceMock = new ResourceMock
            {
                References = new ReferenceCollectionMock<IResource>()
            };

            _modelFactory = BuildUnitOfWorkFactory();

            _typeControllerMock = new Mock<IResourceTypeController>();

            _linkerMock = new Mock<IResourceLinker>();
            _linkerMock.Setup(l => l.SaveReferencesAsync(It.IsAny<IUnitOfWork>(), It.IsAny<Resource>(), It.IsAny<ResourceEntity>(),
                It.IsAny<Dictionary<Resource, ResourceEntity>>()))
                .ReturnsAsync(Array.Empty<Resource>());

            _initializerMock = new Mock<IResourceInitializer>();
            _initializerMock.Setup(i => i.ExecuteAsync(It.IsAny<IResourceGraph>(), It.IsAny<object>())).ReturnsAsync(new ResourceInitializerResult { InitializedResources = [_resourceMock]});
            _linkerMock.Setup(l => l.SaveRootsAsync(It.IsAny<IUnitOfWork>(), It.IsAny<IReadOnlyList<Resource>>()))
                .ReturnsAsync([_resourceMock]);
            _linkerMock.Setup(l => l.SaveReferencesAsync(It.IsAny<IUnitOfWork>(), It.IsAny<Resource>(), It.IsAny<ResourceEntity>(),
                It.IsAny<Dictionary<Resource, ResourceEntity>>()))
                .ReturnsAsync(Array.Empty<Resource>());

            _graph = new ResourceGraph { TypeController = _typeControllerMock.Object };
            _moduleConfig = new ModuleConfig();
            _moduleConfig.Initialize();

            _resourceManager = new ResourceManager
            {
                UowFactory = _modelFactory,
                ResourceLinker = _linkerMock.Object,
                TypeController = _typeControllerMock.Object,
                Graph = _graph,
                Config = _moduleConfig,
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
            };

            _typeControllerMock.Setup(tc => tc.Create(typeof(ResourceMock).ResourceType())).Returns(_resourceMock);
            _typeControllerMock.Setup(tc => tc.Create(typeof(PublicResourceMock).ResourceType())).Returns(new PublicResourceMock()
            {
                References = new ReferenceCollectionMock<IResource>()
            });

            using var uow = _modelFactory.Create();
            var resourceRepo = uow.GetRepository<IResourceRepository>();
            resourceRepo.Create(DatabaseResourceName, typeof(ResourceMock).ResourceType());
            uow.SaveChanges();
        }

        protected virtual UnitOfWorkFactory<ResourcesContext> BuildUnitOfWorkFactory()
        {
            return new UnitOfWorkFactory<ResourcesContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));
        }

        [Test(Description = "Executes a resource initializer on the manager to add resources")]
        public async Task ExecuteInitializer()
        {
            // Act
            await _resourceManager.ExecuteInitializer(_initializerMock.Object, null);

            // Assert
            _linkerMock.Verify(l => l.SaveRootsAsync(It.IsAny<IUnitOfWork>(), It.IsAny<IReadOnlyList<Resource>>()), Times.Once);
        }

        [Test(Description = "If resource manager starts with filled database, it will initialized with values of database.")]
        public async Task InitializeWithDatabaseEntity()
        {
            // Act
            await _resourceManager.InitializeAsync();

            // Assert
            Assert.That(_resourceMock.InitializeCalls, Is.EqualTo(1), "The resource was not initialized.");
            Assert.That(_resourceMock.Name, Is.EqualTo(DatabaseResourceName), "Name does not match to the database entity");
        }

        [Test(Description = "Start call to ResourceManager starts the handled resources.")]
        public async Task StartStartsResources()
        {
            // Arrange
            await _resourceManager.InitializeAsync();

            // Act
            await _resourceManager.StartAsync();

            // Assert
            Assert.That(_resourceMock.InitializeCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StartCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StopCalls, Is.EqualTo(0));
        }

        [Test(Description = "Stop call to ResourceManager stops the handled resources.")]
        public async Task StopStopsResources()
        {
            // Arrange
            await _resourceManager.InitializeAsync();
            await _resourceManager.StartAsync();

            // Act
            await _resourceManager.StopAsync();

            // Assert
            Assert.That(_resourceMock.InitializeCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StartCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StopCalls, Is.EqualTo(1));
        }

        [Test(Description = "ResourceManager is attached to all reference collections and and listens to changed events.")]
        public async Task AutoSaveCollectionsWillBeSaved()
        {
            // Arrange
            var collectionProperty = typeof(IReferenceResource).GetProperty(nameof(IReferenceResource.References));
            _linkerMock.Setup(l => l.SaveSingleCollectionAsync(It.IsAny<IUnitOfWork>(), _resourceMock, collectionProperty))
                .ReturnsAsync(Array.Empty<Resource>);

            var waitEvent = new ManualResetEvent(false);
            _resourceManager.ResourceChanged += (sender, resource) =>
            {
                waitEvent.Set();
            };

            var referenceCollectionMock = new ReferenceCollectionMock<IResource>();
            _resourceMock.References = referenceCollectionMock;

            await _resourceManager.InitializeAsync();
            await _resourceManager.StartAsync();

            // Act
            var eventArgs = new ReferenceCollectionChangedEventArgs(_resourceMock, collectionProperty);
            referenceCollectionMock.RaiseCollectionChanged(eventArgs);

            // Assert
            waitEvent.WaitOne(1000);
            _linkerMock.Verify(l => l.SaveSingleCollectionAsync(It.IsAny<IUnitOfWork>(), _resourceMock, collectionProperty), Times.Once);
        }

        [Test(Description = "Saving resources should save to database")]
        public async Task SaveResource()
        {
            // Arrange
            await _resourceManager.InitializeAsync();
            await _resourceManager.StartAsync();
            _linkerMock.Invocations.Clear();

            _resourceMock.Name = "A Resource Description";

            // Act
            await _resourceManager.SaveAsync(_resourceMock);

            // Assert
            _linkerMock.Verify(l => l.SaveReferencesAsync(It.IsAny<IUnitOfWork>(), _resourceMock, It.IsAny<ResourceEntity>(),
                It.IsAny<Dictionary<Resource, ResourceEntity>>()), Times.Once);

            using var uow = _modelFactory.Create();
            var resourceRepo = uow.GetRepository<IResourceRepository>();
            var entity = await resourceRepo.GetByKeyAsync(_resourceMock.Id);

            Assert.That(entity.Description, Is.EqualTo(_resourceMock.Description));
        }

        [Test(Description = "Resources should be saved on Changed event")]
        public async Task SaveResourceOnResourceChanged()
        {
            // Arrange
            await _resourceManager.InitializeAsync();
            await _resourceManager.StartAsync();

            var testResource = _graph.Instantiate<PublicResourceMock>();
            await _resourceManager.SaveAsync(testResource);
            _linkerMock.Invocations.Clear();

            var waitEvent = new ManualResetEvent(false);
            _resourceManager.ResourceChanged += (sender, resource) =>
            {
                waitEvent.Set();
            };

            // Act
            testResource.Name = "Hello World";
            testResource.RaiseChanged();

            // Assert
            waitEvent.WaitOne(1000);
            _linkerMock.Verify(l => l.SaveReferencesAsync(It.IsAny<IUnitOfWork>(), testResource, It.IsAny<ResourceEntity>(),
                It.IsAny<Dictionary<Resource, ResourceEntity>>()), Times.Once);

            ResourceEntity entity;
            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceRepository>();
                entity = await resourceRepo.GetByKeyAsync(testResource.Id);
            }

            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.Name, Is.EqualTo(testResource.Name));
        }

        [Test(Description = "Should notify facade listeners when resource Changed")]
        public async Task RaiseResourceChangesOnChangedResource()
        {
            // Arrange
            var testResource = _graph.Instantiate<PublicResourceMock>();
            int notifications = 0;
            var waitEvent = new ManualResetEvent(false);
            _resourceManager.ResourceChanged += (sender, resource) =>
            {
                notifications = +1;
                waitEvent.Set();
            };
            await _resourceManager.SaveAsync(testResource);

            // Act
            testResource.Name = "Hello World";
            testResource.RaiseChanged();

            // Assert
            waitEvent.WaitOne(1000);
            Assert.That(notifications, Is.EqualTo(1));
        }

        [Test(Description = "Should not notify facade listeners when new resource added")]
        public async Task DontRaiseResourceChangesOnAdded()
        {
            // Arrange
            var testResource = _graph.Instantiate<PublicResourceMock>();
            var notifications = 0;
            var waitEvent = new ManualResetEvent(false);

            _resourceManager.ResourceChanged += (sender, resource) =>
            {
                notifications = +1;
                waitEvent.Set();
            };

            // Act
            await _resourceManager.SaveAsync(testResource);

            // Assert
            waitEvent.WaitOne(1000);
            Assert.That(notifications, Is.EqualTo(0));
        }


        [Test(Description = "Adds a resource while the ResourceManager was initialized but not started")]
        public async Task AddResourceWhileInitializedDoesNotStartResource()
        {
            // Arrange
            await _resourceManager.InitializeAsync();
            var testResource = _graph.Instantiate<PublicResourceMock>();

            // Act
            await _resourceManager.SaveAsync(testResource);

            // Arrange
            Assert.That(testResource.InitializeCalls, Is.EqualTo(1));
            Assert.That(testResource.StartCalls, Is.EqualTo(0));
        }

        [TestCase(true, Description = "")]
        [TestCase(false, Description = "")]
        public async Task DestroyResource(bool permanent)
        {
            // Arrange
            await _resourceManager.InitializeAsync();
            await _resourceManager.StartAsync();

            var testResource = _graph.Instantiate<PublicResourceMock>();
            await _resourceManager.SaveAsync(testResource);

            // Act
            await _resourceManager.Destroy(testResource, permanent);

            // Assert
            Assert.That(testResource.StopCalls, Is.EqualTo(1));

            _typeControllerMock.Verify(t => t.Destroy(testResource), Times.Once);

            Assert.Throws<ResourceNotFoundException>(() => _graph.GetResource<PublicResourceMock>());

            using var uow = _modelFactory.Create();
            var resourceRepo = uow.GetRepository<IResourceRepository>();

            var entity = await resourceRepo.GetByKeyAsync(testResource.Id);
            if (permanent)
            {
                Assert.That(entity, Is.Null);
            }
            else
            {
                Assert.That(entity, Is.Not.Null);
                Assert.That(entity.Deleted, Is.Not.Null);
            }
        }

        public interface IReferenceResource
        {
            IReferences<IResource> References { get; set; }
        }

        private abstract class ResourceMockBase : Resource
        {
            public int InitializeCalls { get; private set; }

            public int StartCalls { get; private set; }

            public int StopCalls { get; private set; }

            [ResourceReference(ResourceRelationType.Extension, AutoSave = true)]
            public IReferences<IResource> References { get; set; }

            protected override async Task OnInitializeAsync()
            {
                await base.OnInitializeAsync();
                InitializeCalls++;
            }

            protected override async Task OnStartAsync()
            {
                await base.OnStartAsync();
                StartCalls++;
            }

            protected override Task OnStopAsync()
            {
                StopCalls++;
                return base.OnStopAsync();
            }

            public void RaiseChanged()
            {
                RaiseResourceChanged();
            }
        }

        private class PublicResourceMock : ResourceMockBase, IResource, IReferenceResource
        {
            public ICapabilities Capabilities { get; private set; }

            protected override async Task OnInitializeAsync()
            {
                await base.OnInitializeAsync();
                Capabilities = new TestCapabilities();
            }

            public event EventHandler<ICapabilities> CapabilitiesChanged;

        }

        private class ResourceMock : ResourceMockBase
        {

        }

        private class TestCapabilities : ICapabilities
        {
            public bool IsCombined => false;

            public bool ProvidedBy(ICapabilities provided) => provided is TestCapabilities;

            public bool Provides(ICapabilities required) => required is TestCapabilities;

            public IEnumerable<ICapabilities> GetAll()
            {
                yield return this;
            }
        }
    }
}
