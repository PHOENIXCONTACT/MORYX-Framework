// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Resources.Model;
using Moryx.Serialization;
using Moq;
using Moryx.Model.Repositories;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Moryx.Resources.Management.Tests
{
    [TestFixture]
    public class ResourceEntityAccessorTests
    {
        private IResourceTypeController _typeControllerMock;
        private IResourceGraph _resourceGraph;

        [SetUp]
        public void Setup()
        {
            var typeControllerMock = new Mock<IResourceTypeController>();
            typeControllerMock.Setup(tc => tc.Create(It.IsAny<string>())).Returns(new TestResource());

            _typeControllerMock = typeControllerMock.Object;

            var resourceCreator = new Mock<IResourceGraph>();
            _resourceGraph = resourceCreator.Object;
        }

        [Test(Description = "Instantiates a resource")]
        public void InstantiateCreatesAValidResourceObject()
        {
            // Arrange
            var accessor = new ResourceEntityAccessor
            {
                Id = 10,
                Type = typeof(TestResource).ResourceType(),
                Name = nameof(ResourceEntityAccessor.Name),
                Description = nameof(ResourceEntityAccessor.Description),
                ExtensionData = JsonConvert.SerializeObject(new TestResource { Data = new ExtensionDataInherited() }, JsonSettings.Minimal)
            };

            // Act
            var resource = accessor.Instantiate(_typeControllerMock, _resourceGraph) as TestResource;

            // Assert
            Assert.NotNull(resource);
            Assert.AreEqual(10, resource.Id);
            Assert.AreEqual(accessor.Type, resource.GetType().ResourceType());
            Assert.AreEqual(accessor.Name, resource.Name);
            Assert.AreEqual(accessor.Description, resource.Description);

            Assert.NotNull(resource.Data);
            Assert.AreEqual(typeof(ExtensionDataInherited), resource.Data.GetType());
            Assert.AreEqual(1, resource.Data.Value1);
            Assert.AreEqual("MyVal", resource.Data.Value2);
            Assert.AreEqual(42, ((ExtensionDataInherited)resource.Data).Value3);
        }

        [TestCase(false, Description = "Updates an existing ResourceEntity")]
        [TestCase(true, Description = "Creates a new ResourceEntity")]
        public void SaveEntityReturnsAValidResource(bool createNew)
        {
            // Arrange
            var id = createNew ? 0 : 10;
            var type = createNew ? typeof(TestResource).ResourceType() : "";
            const string name = nameof(ResourceEntityAccessor.Name);
            const string description = nameof(ResourceEntityAccessor.Description);

            var entity = new ResourceEntity
            {
                Id = id,
                Type = type,
                Name = "",
                Description = ""
            };

            var repoMock = new Mock<IRepository<ResourceEntity>>();
            repoMock.Setup(r => r.Create()).Returns(entity);
            repoMock.Setup(r => r.GetByKey(It.IsAny<long>())).Returns(entity);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.GetRepository<IRepository<ResourceEntity>>()).Returns(repoMock.Object);

            var resource = new TestResource
            {
                Name = name,
                Data = new ExtensionDataInherited(),
                Description = description
            };

            var extensionDataJson = JsonConvert.SerializeObject(resource, JsonSettings.Minimal);

            // Act
            var resourceEntity = ResourceEntityAccessor.SaveToEntity(unitOfWorkMock.Object, resource).Item1;

            // Assert
            Assert.NotNull(resourceEntity);
            Assert.AreEqual(id, resourceEntity.Id);
            Assert.AreEqual(type, resourceEntity.Type);
            Assert.AreEqual(name, resourceEntity.Name);
            Assert.AreEqual(description, resourceEntity.Description);
            Assert.AreEqual(extensionDataJson, resourceEntity.ExtensionData);
        }

        private class ExtensionDataTestBase
        {
            public int Value1 => 1;

            public string Value2 => "MyVal";
        }

        private class ExtensionDataInherited : ExtensionDataTestBase
        {
            public long Value3 => 42;
        }

        private class TestResource : Resource
        {
            [DataMember]
            public ExtensionDataTestBase Data { get; set; }
        }
    }
}
