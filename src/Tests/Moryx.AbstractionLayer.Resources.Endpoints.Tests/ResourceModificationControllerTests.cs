// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Resources.Endpoints.Models;
using Moryx.AbstractionLayer.TestTools;
using Moryx.AbstractionLayer.TestTools.Resources;
using Moryx.Runtime.Modules;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Resources.Endpoints.Tests;

[TestFixture]
internal class ResourceModificationControllerTests
{
    private Mock<IResourceManagement> _resourceManagementMock;
    private Mock<IResourceTypeTree> _resourceTypeTreeMock;
    private Mock<IModuleManager> _moduleManagerMock;
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IResourceTypeNode> _nodeMock;
    private ResourceModificationController _controller;
    private ReferenceResource _resource;
    private ResourceQuery _query;
    private ReferenceCollectionMock<ISimpleResource> _references;

    [SetUp]
    public void SetUp()
    {
        _resourceManagementMock = new Mock<IResourceManagement>();
        _resourceTypeTreeMock = new Mock<IResourceTypeTree>();
        _moduleManagerMock = new Mock<IModuleManager>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _nodeMock = new Mock<IResourceTypeNode>();
        _references = new ReferenceCollectionMock<ISimpleResource>();

        _resource = new ReferenceResource
        {
            References = _references,
        };

        _query = new ResourceQuery();
        _nodeMock.SetupGet(node => node.ResourceType)
            .Returns(typeof(ReferenceResource));
        _nodeMock.SetupGet(node => node.PropertiesOfResourceType)
            .Returns(typeof(ReferenceResource).GetProperties());
        _resourceTypeTreeMock.Setup(tree => tree[typeof(ReferenceResource).FullName])
            .Returns(_nodeMock.Object);
        _resourceManagementMock
            .Setup(management => management.GetResourcesUnsafe<IResource>(
                       It.IsAny<Func<IResource, bool>>()))
            .Returns((Func<IResource, bool> predicate) =>
                         predicate(_resource) ? new IResource[] { _resource }
                                              : Array.Empty<IResource>());
        _resourceManagementMock
            .Setup(management => management.ReadUnsafe<ResourceModel>(
                       It.IsAny<long>(), It.IsAny<Func<Resource, ResourceModel>>()))
            .Returns((long _,
                      Func<Resource, ResourceModel> converter) => converter(_resource));
        var facadeContainerMock = new Mock<IServerModule>();
        facadeContainerMock.As<IFacadeContainer<IResourceManagement>>()
            .Setup(container => container.Facade)
            .Returns(_resourceManagementMock.Object);
        _moduleManagerMock
            .Setup(manager => manager.AllModules)
            .Returns(new[] { facadeContainerMock.Object });
        _controller = new ResourceModificationController(
            _resourceManagementMock.Object, _resourceTypeTreeMock.Object,
            _moduleManagerMock.Object, _serviceProviderMock.Object);
    }

    [Test]
    public void GetResources_IrrelevantReferenceConstraint_ReturnsResources()
    {
        // Arrange
        _query.ReferenceCondition = new ReferenceFilter
        {
            Name = nameof(Resource.Children),
            ValueConstraint = ReferenceValue.Irrelevant
        };
        // Act
        var result = _controller.GetResources(_query);
        // Assert
        Assert.That(result.Value, Has.Length.EqualTo(1));
        Assert.That(result.Value[0].Id, Is.EqualTo(_resource.Id));
    }

    [Test]
    public void GetResources_ReferenceCollectionIsEmptyAndConstraintIsNotEmpty_ReturnsEmpty()
    {
        // Arrange
        _query.ReferenceCondition = new ReferenceFilter
        {
            Name = nameof(ReferenceResource.References),
            ValueConstraint = ReferenceValue.NotEmpty
        };
        // Act
        var result = _controller.GetResources(_query);
        // Assert
        Assert.That(result.Value, Is.Empty);
    }

    [Test]
    public void GetResources_ReferenceCollectionIsNotEmptyAndConstraintIsNotEmpty_ReturnsResources()
    {
        // Arrange
        _references.Add(new SimpleResource());
        _query.ReferenceCondition = new ReferenceFilter
        {
            Name = nameof(ReferenceResource.References),
            ValueConstraint = ReferenceValue.NotEmpty
        };
        // Act
        var result = _controller.GetResources(_query);
        // Assert
        var resources = result.Value!;
        Assert.That(resources, Has.Length.EqualTo(1));
        Assert.That(resources[0].Id, Is.EqualTo(_resource.Id));
    }

    [Test]
    public void GetResources_ReferenceCollectionIsEmptyAndConstraintIsNullOrEmpty_ReturnsResources()
    {
        // Arrange
        _query.ReferenceCondition = new ReferenceFilter
        {
            Name = nameof(ReferenceResource.References),
            ValueConstraint = ReferenceValue.NullOrEmpty
        };
        // Act
        var result = _controller.GetResources(_query);
        // Assert
        var resources = result.Value!;
        Assert.That(resources, Has.Length.EqualTo(1));
        Assert.That(resources[0].Id, Is.EqualTo(_resource.Id));
    }

    [Test]
    public void GetResources_ReferenceCollectionIsNotEmptyAndConstraintIsNullOrEmpty_ReturnsEmpty()
    {
        // Arrange
        _references.Add(new SimpleResource());
        _query.ReferenceCondition = new ReferenceFilter
        {
            Name = nameof(ReferenceResource.References),
            ValueConstraint = ReferenceValue.NullOrEmpty
        };
        // Act
        var result = _controller.GetResources(_query);
        // Assert
        Assert.That(result.Value, Is.Empty);
    }

}
