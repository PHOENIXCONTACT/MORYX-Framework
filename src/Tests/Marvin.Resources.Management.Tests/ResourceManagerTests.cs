using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;
using Marvin.AbstractionLayer.TestTools;
using Marvin.Model;
using Marvin.Resources.Model;
using Marvin.TestTools.UnitTest;
using Moq;
using NUnit.Framework;

namespace Marvin.Resources.Management.Tests
{
    [TestFixture]
    public class ResourceManagerTests
    {
        private IUnitOfWorkFactory _modelFactory;
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

            var modelFactory = new InMemoryUnitOfWorkFactory(Guid.NewGuid().ToString());
            modelFactory.Initialize();
            _modelFactory = modelFactory;

            _typeControllerMock = new Mock<IResourceTypeController>();

            _linkerMock = new Mock<IResourceLinker>();
            _linkerMock.Setup(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), It.IsAny<Resource>(), It.IsAny<ResourceEntity>()))
                .Returns(new Resource[0]);

            _initializerMock = new Mock<IResourceInitializer>();
            _initializerMock.Setup(i => i.Execute(It.IsAny<IResourceGraph>())).Returns(new[] { _resourceMock });
            _linkerMock.Setup(l => l.SaveRoots(It.IsAny<IUnitOfWork>(), It.IsAny<IReadOnlyList<Resource>>()))
                .Returns(new[] { _resourceMock });
            _linkerMock.Setup(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), It.IsAny<Resource>(), It.IsAny<ResourceEntity>()))
                .Returns(new Resource[0]);

            _graph = new ResourceGraph { TypeController = _typeControllerMock.Object };
            _resourceManager = new ResourceManager
            {
                UowFactory = _modelFactory,
                ResourceLinker = _linkerMock.Object,
                TypeController = _typeControllerMock.Object,
                Graph = _graph,
                Logger = new DummyLogger()
            };

            _typeControllerMock.Setup(tc => tc.Create(typeof(ResourceMock).ResourceType())).Returns(_resourceMock);
            _typeControllerMock.Setup(tc => tc.Create(typeof(PublicResourceMock).ResourceType())).Returns(new PublicResourceMock()
            {
                References = new ReferenceCollectionMock<IResource>()
            });

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();
                resourceRepo.Create(DatabaseResourceName, typeof(ResourceMock).ResourceType());
                uow.Save();
            }
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
            Assert.AreEqual(1, _resourceMock.InitializeCalls, "The resource was not initialized.");
            Assert.AreEqual(DatabaseResourceName, _resourceMock.Name, "Name does not match to the database entity");
        }

        [Test(Description = "Start call to ResourceManager starts the handled resources.")]
        public void StartStartsResources()
        {
            // Arrange
            _resourceManager.Initialize();

            // Act
            _resourceManager.Start();

            // Assert
            Assert.AreEqual(1, _resourceMock.InitializeCalls);
            Assert.AreEqual(1, _resourceMock.StartCalls);
            Assert.AreEqual(0, _resourceMock.StopCalls);
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
            Assert.AreEqual(1, _resourceMock.InitializeCalls);
            Assert.AreEqual(1, _resourceMock.StartCalls);
            Assert.AreEqual(1, _resourceMock.StopCalls);
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
            _linkerMock.Verify(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), _resourceMock, It.IsAny<ResourceEntity>()), Times.Once);

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();
                var entity = resourceRepo.GetByKey(_resourceMock.Id);

                Assert.AreEqual(_resourceMock.Description, entity.Description);
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
            _linkerMock.Verify(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), testResource, It.IsAny<ResourceEntity>()), Times.Once);

            ResourceEntity entity;
            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();
                entity = resourceRepo.GetByKey(testResource.Id);
            }

            Assert.IsNotNull(entity);
            Assert.AreEqual(testResource.Name, entity.Name);
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
            Assert.AreEqual(1, testResource.InitializeCalls);
            Assert.AreEqual(0, testResource.StartCalls);
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
            Assert.AreEqual(1, testResource.StopCalls);

            _typeControllerMock.Verify(t => t.Destroy(testResource), Times.Once);

            Assert.Throws<ResourceNotFoundException>(() => _graph.GetResource<PublicResourceMock>());

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();

                var entity = resourceRepo.GetByKey(testResource.Id);
                if (permanent)
                {
                    Assert.IsNull(entity);
                }
                else
                {
                    Assert.IsNotNull(entity);
                    Assert.IsNotNull(entity.Deleted);
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

        private class PublicResourceMock : ResourceMockBase, IPublicResource, IReferenceResource
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