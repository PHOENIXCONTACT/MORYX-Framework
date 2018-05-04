using System;
using System.Collections.Generic;
using System.Data.Entity;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;
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

        private ResourceManager _resourceManager;
        private Mock<IResourceInitializer> _initializerMock;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            DbConfiguration.SetConfiguration(new EntityFrameworkConfiguration());
        }

        [SetUp]
        public void Setup()
        {
            _resourceMock = new ResourceMock {Name = "ResourceMock"};

            var modelFactory = new InMemoryUnitOfWorkFactory(Guid.NewGuid().ToString());
            modelFactory.Initialize();
            _modelFactory = modelFactory;

            _typeControllerMock = new Mock<IResourceTypeController>();

            _linkerMock = new Mock<IResourceLinker>();
            _linkerMock.Setup(l => l.GetAutoSaveCollections(It.IsAny<Resource>())).Returns(new List<IReferenceCollection>());

            _initializerMock = new Mock<IResourceInitializer>();
            _initializerMock.Setup(i => i.Execute(It.IsAny<IResourceCreator>())).Returns(new[] { _resourceMock });
            _linkerMock.Setup(l => l.SaveRoots(It.IsAny<IUnitOfWork>(), It.IsAny<IReadOnlyList<Resource>>()))
                .Returns(new[] { _resourceMock });

            _resourceManager = new ResourceManager
            {
                UowFactory = _modelFactory,
                ResourceLinker = _linkerMock.Object,
                TypeController = _typeControllerMock.Object,
                Logger = new DummyLogger()
            };

            _typeControllerMock.Setup(tc => tc.Create(nameof(ResourceMock))).Returns(_resourceMock);
            _typeControllerMock.Setup(tc => tc.Create(nameof(PublicResourceMock))).Returns(new PublicResourceMock());

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();
                resourceRepo.Create(nameof(ResourceMock), nameof(ResourceMock));
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
            Assert.AreEqual(1, _resourceMock.InitializeCalls);
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

            var referenceCollectionMock = new Mock<IReferenceCollection>();
            _linkerMock.Setup(l => l.GetAutoSaveCollections(_resourceMock)).Returns(
                new List<IReferenceCollection>
                {
                    referenceCollectionMock.Object
                });

            _resourceManager.Initialize();
            _resourceManager.Start();

            // Act
            var eventArgs = new ReferenceCollectionChangedEventArgs(_resourceMock, collectionProperty);
            referenceCollectionMock.Raise(rc => rc.CollectionChanged += null, referenceCollectionMock.Object, eventArgs);

            // Assert
            _linkerMock.Verify(l => l.SaveSingleCollection(It.IsAny<IUnitOfWork>(), _resourceMock, collectionProperty), Times.Once);
        }

        [Test(Description = "Saving resources should save to database")]
        public void SaveResource()
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();
            _linkerMock.ResetCalls();

            // Act
            _resourceMock.LocalIdentifier = "Local";
            _resourceManager.Save(_resourceMock);

            // Assert
            _linkerMock.Verify(l => l.SaveReferences(It.IsAny<IUnitOfWork>(), _resourceMock, It.IsAny<ResourceEntity>()), Times.Once);

            using (var uow = _modelFactory.Create())
            {
                var resourceRepo = uow.GetRepository<IResourceEntityRepository>();
                var entity = resourceRepo.GetByKey(_resourceMock.Id);

                Assert.AreEqual(_resourceMock.LocalIdentifier, entity.LocalIdentifier);
            }
        }

        [Test(Description = "Resources should be saved on Changed event")]
        public void SaveResourceOnResouceChanged()
        {
            // Arrange
            _resourceManager.Initialize();
            _resourceManager.Start();
            
            var testResource = _resourceManager.Instantiate<PublicResourceMock>();
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
            var testResource = _resourceManager.Instantiate<PublicResourceMock>();

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

            var testResource = _resourceManager.Instantiate<PublicResourceMock>();
            _resourceManager.Save(testResource);

            // Act
            _resourceManager.Destroy(testResource, permanent);

            // Assert
            Assert.AreEqual(1, testResource.StopCalls);

            _typeControllerMock.Verify(t => t.Destroy(testResource), Times.Once);

            Assert.Throws<ResourceNotFoundException>(() => _resourceManager.GetResource<PublicResourceMock>(new TestCapabilities()));

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

        private class PublicResourceMock : ResourceMockBase, IPublicResource, IReferenceResource
        {
            public ICapabilities Capabilities { get; private set; }

            public override void Initialize()
            {
                base.Initialize();
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