// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Resources.Model;
using Moq;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Logging;

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
            _linkerMock.Setup(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), It.IsAny<Resource>(), It.IsAny<ResourceEntity>(), 
                It.IsAny<Dictionary<Resource,ResourceEntity>>()))
                .Returns(new Resource[0]);

            _initializerMock = new Mock<IResourceInitializer>();
            _initializerMock.Setup(i => i.Execute(It.IsAny<IResourceGraph>())).Returns(new[] { _resourceMock });
            _linkerMock.Setup(l => l.SaveRoots(It.IsAny<IUnitOfWork>(), It.IsAny<IReadOnlyList<Resource>>()))
                .Returns(new[] { _resourceMock });
            _linkerMock.Setup(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), It.IsAny<Resource>(), It.IsAny<ResourceEntity>(),
                It.IsAny<Dictionary<Resource, ResourceEntity>>()))
                .Returns(new Resource[0]);

            _graph = new ResourceGraph { TypeController = _typeControllerMock.Object };
            _resourceManager = new ResourceManager
            {
                UowFactory = _modelFactory,
                ResourceLinker = _linkerMock.Object,
                TypeController = _typeControllerMock.Object,
                Graph = _graph,
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
        public void ExecuteInitializer()
        {
            // Act
            _resourceManager.ExecuteInitializer(_initializerMock.Object);

            // Assert
            _linkerMock.Verify(l => l.SaveRoots(It.IsAny<IUnitOfWork>(), It.IsAny<IReadOnlyList<Resource>>()), Times.Once);
        }

        [Test(Description = "If resource manager starts with filled database, it will initialized with values of database.")]
        public void InitializeWithDatabaseEntity()
        {
            // Act
            _resourceManager.Initialize();

            // Assert
            Assert.That(_resourceMock.InitializeCalls, Is.EqualTo(1), "The resource was not initialized.");
            Assert.That(_resourceMock.Name, Is.EqualTo(DatabaseResourceName), "Name does not match to the database entity");
        }

        [Test(Description = "Start call to ResourceManager starts the handled resources.")]
        public void StartStartsResources()
        {
            // Arrange
            _resourceManager.Initialize();

            // Act
            _resourceManager.Start();

            // Assert
            Assert.That(_resourceMock.InitializeCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StartCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StopCalls, Is.EqualTo(0));
        }

        [Test(Description = "Stop call to ResourceManager stops the handled resources.")]
        public void StopStopsResources()
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();

            // Act
            _resourceManager.Stop();

            // Assert
            Assert.That(_resourceMock.InitializeCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StartCalls, Is.EqualTo(1));
            Assert.That(_resourceMock.StopCalls, Is.EqualTo(1));
        }

        [Test(Description = "ResourceManager is attached to all reference collections and and listens to changed events.")]
        public void AutoSaveCollectionsWillBeSaved()
        {
            // Arrange
            var collectionProperty = typeof(IReferenceResource).GetProperty(nameof(IReferenceResource.References));
            _linkerMock.Setup(l => l.SaveSingleCollection(It.IsAny<IUnitOfWork>(), _resourceMock, collectionProperty))
                .Returns(() => new Resource[0]);

            var referenceCollectionMock = new ReferenceCollectionMock<IResource>();
            _resourceMock.References = referenceCollectionMock;

            _resourceManager.Initialize();
            _resourceManager.Start();

            // Act
            var eventArgs = new ReferenceCollectionChangedEventArgs(_resourceMock, collectionProperty);
            referenceCollectionMock.RaiseCollectionChanged(eventArgs);

            // Assert
            _linkerMock.Verify(l => l.SaveSingleCollection(It.IsAny<IUnitOfWork>(), _resourceMock, collectionProperty), Times.Once);
        }

        [Test(Description = "Saving resources should save to database")]
        public void SaveResource()
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();
            _linkerMock.Invocations.Clear();

            _resourceMock.Name = "A Resource Description";

            // Act
            _resourceManager.Save(_resourceMock);

            // Assert
            _linkerMock.Verify(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), _resourceMock, It.IsAny<ResourceEntity>(),
                It.IsAny<Dictionary<Resource, ResourceEntity>>()), Times.Once);

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceRepository>();
                var entity = resourceRepo.GetByKey(_resourceMock.Id);

                Assert.That(entity.Description, Is.EqualTo(_resourceMock.Description));
            }
        }

        [Test(Description = "Resources should be saved on Changed event")]
        public void SaveResourceOnResourceChanged()
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();

            var testResource = _graph.Instantiate<PublicResourceMock>();
            _resourceManager.Save(testResource);
            _linkerMock.Invocations.Clear();

            // Act
            testResource.Name = "Hello World";
            testResource.RaiseChanged();

            // Assert
            _linkerMock.Verify(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), testResource, It.IsAny<ResourceEntity>()
                , It.IsAny<Dictionary<Resource, ResourceEntity>>()), Times.Once);

            ResourceEntity entity;
            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceRepository>();
                entity = resourceRepo.GetByKey(testResource.Id);
            }

            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.Name, Is.EqualTo(testResource.Name));
        }

        [Test(Description = "Adds a resource while the ResourceManager was initialized but not started")]
        public void AddResourceWhileInitializedDoesNotStartResource()
        {
            // Arrange
            _resourceManager.Initialize();
            var testResource = _graph.Instantiate<PublicResourceMock>();

            // Act
            _resourceManager.Save(testResource);

            // Arrange
            Assert.That(testResource.InitializeCalls, Is.EqualTo(1));
            Assert.That(testResource.StartCalls, Is.EqualTo(0));
        }

        [TestCase(true, Description = "")]
        [TestCase(false, Description = "")]
        public void DestroyResource(bool permanent)
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();

            var testResource = _graph.Instantiate<PublicResourceMock>();
            _resourceManager.Save(testResource);

            // Act
            _resourceManager.Destroy(testResource, permanent);

            // Assert
            Assert.That(testResource.StopCalls, Is.EqualTo(1));

            _typeControllerMock.Verify(t => t.Destroy(testResource), Times.Once);

            Assert.Throws<ResourceNotFoundException>(() => _graph.GetResource<PublicResourceMock>());

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceRepository>();

                var entity = resourceRepo.GetByKey(testResource.Id);
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

            protected override void OnInitialize()
            {
                base.OnInitialize();
                InitializeCalls++;
            }

            protected override void OnStart()
            {
                base.OnStart();
                StartCalls++;
            }

            protected override void OnStop()
            {
                base.OnStop();
                StopCalls++;
            }

            public void RaiseChanged()
            {
                RaiseResourceChanged();
            }
        }

        private class PublicResourceMock : ResourceMockBase, IResource, IReferenceResource
        {
            public ICapabilities Capabilities { get; private set; }

            protected override void OnInitialize()
            {
                base.OnInitialize();
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
