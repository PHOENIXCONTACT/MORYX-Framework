using System;
using System.Collections.Generic;
using System.Data.Entity;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;
using Marvin.Model;
using Marvin.Resources.Model;
using Moq;
using NUnit.Framework;

namespace Marvin.Resources.Management.Tests
{
    [TestFixture]
    public class ResourceManagerTests
    {
        private IUnitOfWorkFactory _modelFactory;
        private ModuleConfig _moduleConfig;
        private Mock<IResourceTypeController> _typeControllerMock;
        private Mock<IResourceLinker> _linkerMock;
        private DefaultResourceMock _defaultResource;

        private ResourceManager _resourceManager;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            DbConfiguration.SetConfiguration(new EntityFrameworkConfiguration());
        }

        [SetUp]
        public void Setup()
        {
            var modelFactory = new InMemoryUnitOfWorkFactory(Guid.NewGuid().ToString());
            modelFactory.Initialize();
            _modelFactory = modelFactory;

            _moduleConfig = new ModuleConfig
            {
                DefaultResource = nameof(DefaultResourceMock)
            };
            _typeControllerMock = new Mock<IResourceTypeController>();

            _linkerMock = new Mock<IResourceLinker>();
            _linkerMock.Setup(l => l.GetAutoSaveCollections(It.IsAny<Resource>())).Returns(new List<IReferenceCollection>());

            _resourceManager = new ResourceManager
            {
                UowFactory = _modelFactory,
                Config = _moduleConfig,
                ResourceLinker = _linkerMock.Object,
                TypeController = _typeControllerMock.Object
            };

            _defaultResource = new DefaultResourceMock();

            _typeControllerMock.Setup(tc => tc.Create(_moduleConfig.DefaultResource)).Returns(_defaultResource);
            _typeControllerMock.Setup(tc => tc.Create(nameof(ResourceMock))).Returns(new ResourceMock());
        }

        
        [Test(Description = "If resource manager starts with empty database, the configured RootType will be created and initialized")]
        public void InitializeWithEmptyDatabaseCreatesRoot()
        {
            // Arrange
            // Act
            _resourceManager.Initialize();

            // Assert
            _typeControllerMock.Verify(tc => tc.Create(_moduleConfig.DefaultResource), Times.Once);
            _linkerMock.Verify(l => l.GetAutoSaveCollections(_defaultResource), Times.Once);
            Assert.AreEqual(1, _defaultResource.InitializeCalls);
            Assert.AreEqual(0, _defaultResource.StartCalls);
            Assert.AreEqual(0, _defaultResource.StopCalls);
        }

        [Test(Description = "If resource manager starts with filled database, it will initialized with values of database.")]
        public void InitializeWithDatabaseEntity()
        {
            // Arrange
            const string globalIdentifier = "TestCase";
            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();
                var entity = resourceRepo.Create(_moduleConfig.DefaultResource, _moduleConfig.DefaultResource);
                entity.GlobalIdentifier = globalIdentifier;

                uow.Save();
            }

            // Act
            _resourceManager.Initialize();

            // Assert
            Assert.AreEqual(1, _defaultResource.InitializeCalls);
            Assert.AreEqual(globalIdentifier, _defaultResource.GlobalIdentifier);
        }

        [Test(Description = "Start call to ResourceManager starts the handled resources.")]
        public void StartStartsResources()
        {
            // Arrange
            _resourceManager.Initialize();

            // Act
            _resourceManager.Start();

            // Assert
            Assert.AreEqual(1, _defaultResource.InitializeCalls);
            Assert.AreEqual(1, _defaultResource.StartCalls);
            Assert.AreEqual(0, _defaultResource.StopCalls);
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
            Assert.AreEqual(1, _defaultResource.InitializeCalls);
            Assert.AreEqual(1, _defaultResource.StartCalls);
            Assert.AreEqual(1, _defaultResource.StopCalls);
        }

        [Test(Description = "ResourceManager is attached to all reference collections and and listens to changed events.")]
        public void AutoSaveCollectionsWillBeSaved()
        {
            // Arrange
            var collectionProperty = typeof(IReferenceResource).GetProperty(nameof(IReferenceResource.References));
            _linkerMock.Setup(l => l.SaveSingleCollection(It.IsAny<IUnitOfWork>(), _defaultResource, collectionProperty))
                .Returns(() => new Resource[0]);

            var referenceCollectionMock = new Mock<IReferenceCollection>();
            _linkerMock.Setup(l => l.GetAutoSaveCollections(_defaultResource)).Returns(
                new List<IReferenceCollection>
                {
                    referenceCollectionMock.Object
                });

            _resourceManager.Initialize();
            _resourceManager.Start();

            // Act
            var eventArgs = new ReferenceCollectionChangedEventArgs(_defaultResource, collectionProperty);
            referenceCollectionMock.Raise(rc => rc.CollectionChanged += null, referenceCollectionMock.Object, eventArgs);

            // Assert
            _linkerMock.Verify(l => l.SaveSingleCollection(It.IsAny<IUnitOfWork>(), _defaultResource, collectionProperty), Times.Once);
        }

        [Test(Description = "Saving resources should save to database")]
        public void SaveResource()
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();
            _linkerMock.ResetCalls();

            // Act
            _defaultResource.LocalIdentifier = "Local";
            _resourceManager.Save(_defaultResource);

            // Assert
            _linkerMock.Verify(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), _defaultResource, It.IsAny<ResourceEntity>()), Times.Once);

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();
                var entity = resourceRepo.GetByKey(_defaultResource.Id);

                Assert.AreEqual(_defaultResource.LocalIdentifier, entity.LocalIdentifier);
            }
        }

        [Test(Description = "Resources should be saved on Changed event")]
        public void SaveResourceOnResouceChanged()
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();
            
            var testResource = _resourceManager.Instantiate<ResourceMock>();
            _resourceManager.Save(testResource);
            _linkerMock.ResetCalls();

            // Act
            testResource.GlobalIdentifier = "Hello World";
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
            Assert.AreEqual(testResource.GlobalIdentifier, entity.GlobalIdentifier);
        }

        [Test(Description = "Adds a resource while the ResourceManager was initialized but not started")]
        public void AddResourceWhileInitializedDoesNotStartResource()
        {
            // Arrange
            _resourceManager.Initialize();
            var testResource = _resourceManager.Instantiate<ResourceMock>();

            // Act
            _resourceManager.Save(testResource);

            // Arrange
            Assert.AreEqual(1, testResource.InitializeCalls);
            Assert.AreEqual(0, testResource.StartCalls);
        }

        [TestCase(true, Description ="")]
        [TestCase(false, Description = "")]
        public void DestroyResource(bool permanent)
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();

            var testResource = _resourceManager.Instantiate<ResourceMock>();
            _resourceManager.Save(testResource);

            // Act
            _resourceManager.Destroy(testResource, permanent);

            // Assert
            Assert.AreEqual(1, testResource.StopCalls);

            _typeControllerMock.Verify(t => t.Destroy(testResource), Times.Once);

            Assert.Throws<ResourceNotFoundException>(() => _resourceManager.GetResource<ResourceMock>(new TestCapabilities()));

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

            
            public IReferences<IResource> References { get; set; }

            public override void Initialize()
            {
                base.Initialize();
                

                InitializeCalls++;
            }

            public override void Start()
            {
                base.Start();
                StartCalls++;
            }

            public override void Stop()
            {
                base.Stop();
                StopCalls++;
            }

            public void RaiseChanged()
            {
                RaiseResourceChanged();
            }
        }

        private class ResourceMock : ResourceMockBase, IPublicResource, IReferenceResource
        {
            public ICapabilities Capabilities { get; private set; }

            public override void Initialize()
            {
                base.Initialize();
                Capabilities = new TestCapabilities();
            }

            public event EventHandler<ICapabilities> CapabilitiesChanged;
            
        }

        private class DefaultResourceMock : ResourceMockBase, IDefaultResource
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