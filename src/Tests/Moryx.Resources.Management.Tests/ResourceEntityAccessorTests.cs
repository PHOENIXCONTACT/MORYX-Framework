// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Resources.Model;
using Moryx.Serialization;
using Moq;
using Moryx.Model.Repositories;
using Newtonsoft.Json;
using NUnit.Framework;
using System.ComponentModel;

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
            typeControllerMock.Setup(tc => tc.Create(It.Is<string>(type => type == typeof(TestResource).ResourceType()))).Returns(new TestResource());
            typeControllerMock.Setup(tc => tc.Create(It.Is<string>(type => type == typeof(DefaultTestResource).ResourceType()))).Returns(new DefaultTestResource());

            _typeControllerMock = typeControllerMock.Object;

            var resourceCreator = new Mock<IResourceGraph>();
            _resourceGraph = resourceCreator.Object;
        }

        [Test(Description = "Calling Instantiate without an entity sets default value")]
        public void InstantiateWithoutEntitySetsDefaults()
        {
            // Arrange
            var accessor = new ResourceEntityAccessor
            {
                Type = typeof(DefaultTestResource).ResourceType()
            };

            // Act
            var resource = accessor.Instantiate(_typeControllerMock, _resourceGraph) as DefaultTestResource;

            // Assert
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.GetType().ResourceType(), Is.EqualTo(accessor.Type));

            Assert.That(resource.Enabled);
            Assert.That(resource.Number, Is.EqualTo(42));
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
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.Id, Is.EqualTo(10));
            Assert.That(resource.GetType().ResourceType(), Is.EqualTo(accessor.Type));
            Assert.That(resource.Name, Is.EqualTo(accessor.Name));
            Assert.That(resource.Description, Is.EqualTo(accessor.Description));

            Assert.That(resource.Data, Is.Not.Null);
            Assert.That(resource.Data.GetType(), Is.EqualTo(typeof(ExtensionDataInherited)));
            Assert.That(resource.Data.Value1, Is.EqualTo(1));
            Assert.That(resource.Data.Value2, Is.EqualTo("MyVal"));
            Assert.That(((ExtensionDataInherited)resource.Data).Value3, Is.EqualTo(42));
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
            var resourceEntity = ResourceEntityAccessor.SaveToEntity(unitOfWorkMock.Object, resource);

            // Assert
            Assert.That(resourceEntity, Is.Not.Null);
            Assert.That(resourceEntity.Id, Is.EqualTo(id));
            Assert.That(resourceEntity.Type, Is.EqualTo(type));
            Assert.That(resourceEntity.Name, Is.EqualTo(name));
            Assert.That(resourceEntity.Description, Is.EqualTo(description));
            Assert.That(resourceEntity.ExtensionData, Is.EqualTo(extensionDataJson));
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

        private class DefaultTestResource : Resource
        {
            [DataMember, DefaultValue(42)]
            public int Number { get; set; }

            [DataMember, DefaultValue(true)]
            public bool Enabled { get; set; }
        }

        private class TestResource : Resource
        {
            [DataMember]
            public ExtensionDataTestBase Data { get; set; }
        }
    }
}
