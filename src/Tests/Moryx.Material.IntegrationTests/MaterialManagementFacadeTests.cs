// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Lineage;
using Moryx.Material.States;
using NUnit.Framework;

namespace Moryx.Material.IntegrationTests;

[TestFixture]
internal sealed class MaterialManagementFacadeTests : TestBase
{
    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
    }

    [Test]
    public void GetContainerTypes_InTestEnvironment_ReturnsBaseClass()
    {
        // Act
        var containerTypes = _materialManagement.GetContainerTypes();

        // Assert
        Assert.That(containerTypes, Is.EquivalentTo([typeof(BasicMaterialContainer)]));
    }

    [Test]
    public async Task RequestMaterialAsync_WithAvailableContainerType_CreatesContainerInRequestedState()
    {
        // Act
        var requestedContainer = await _materialManagement.RequestMaterialAsync(DummyMaterialRequest, typeof(BasicMaterialContainer));

        // Assert
        var container = _materialManagement.GetContainers().Single();
        var requestedStateInformation = container.StateInformation as RequestedStateInformation;
        Assert.Multiple(() =>
        {
            Assert.That(container, Is.SameAs(requestedContainer));
            Assert.That(container.Id, Is.GreaterThan(0));
            Assert.That(container.Name, Is.EqualTo($"Request-{DummyMaterialRequest.Id}"));
            Assert.That(container.State, Is.EqualTo(StateClassification.Requested));
            Assert.That(container.Material, Is.EqualTo(DummyMaterialRequest.Material));
            Assert.That(container.Quantity, Is.EqualTo(DummyMaterialRequest.RequestedQuantity));
            Assert.That(container.Unit, Is.EqualTo(DummyMaterialRequest.Unit));
            Assert.That(container.Identity, Is.EqualTo(DummyMaterialRequest.ContainerIdentity));
            Assert.That(requestedStateInformation, Is.Not.Null);
            Assert.That(requestedStateInformation!.RequestId, Is.EqualTo(DummyMaterialRequest.Id));
            Assert.That(requestedStateInformation.ExpectedArrival, Is.EqualTo(DummyMaterialRequest.ExpectedArrival));
            Assert.That(requestedStateInformation.IsPartiallyFulfilled, Is.False);
        });
    }

    [Test]
    public void RequestMaterialAsync_WithAbstractContainerType_ThrowsInvalidOperation()
    {
        Assert.That(async () => await _materialManagement.RequestMaterialAsync(DummyMaterialRequest, typeof(MaterialContainer)),
            Throws.InvalidOperationException, "Should not be able to provide abstract container types for creation");
    }

    [Test]
    public async Task PreAdviceMaterialAsync_WithExisitingContainer_TransitionsContainerIntoOutboundState()
    {
        // Arrange
        var container = CreateAvailableDummyMaterialContainer(42);
        var preAdvice = new MaterialPreAdvice
        {
            ContainerId = container.Id,
            DepartureReason = PreAdviceDepartureReason.Transfer
        };
        _resourceManagementMock.Raise(m => m.ResourceAdded += It.IsAny<EventHandler<IResource>>(), this, container);

        // Act
        _ = await _materialManagement.PreAdviceMaterialAsync(preAdvice);

        // Assert
        var result = _materialManagement.GetContainers().Single();
        var outboundStateInformation = container.StateInformation as OutboundStateInformation;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(container));
            Assert.That(result.State, Is.EqualTo(StateClassification.Outbound));
            Assert.That(outboundStateInformation, Is.Not.Null);
            Assert.That(outboundStateInformation!.DepartureReason, Is.EqualTo(preAdvice.DepartureReason));
            Assert.That(outboundStateInformation!.EnteredAt, Is.Not.Null);
        });
    }

    [Test]
    public void GetContainers_WithoutFilter_ReturnsAllContainers()
    {
        Assert.Ignore("Test stub for IMaterialManagement.GetContainers().");
    }

    [Test]
    public void GetContainers_WithFilter_ReturnsMatchingContainers()
    {
        Assert.Ignore("Test stub for IMaterialManagement.GetContainers(Func<IMaterialContainer, bool>).");
    }

    [Test]
    public void GetContainer_WithIdentity_ReturnsMatchingContainer()
    {
        Assert.Ignore("Test stub for IMaterialManagement.GetContainer(IIdentity).");
    }

    [Test]
    public void CancelMaterialRequestAsync_WithPendingRequest_CancelsRequest()
    {
        Assert.Ignore("Test stub for IMaterialManagement.CancelMaterialRequestAsync(Guid, CancellationToken).");
    }

    [Test]
    public void AnnounceMaterialAsync_WithAnnouncement_RecordsInboundMaterial()
    {
        Assert.Ignore("Test stub for IMaterialManagement.AnnounceMaterialAsync(MaterialAnnouncement, CancellationToken).");
    }

    [Test]
    public void DropMaterialAnnouncementAsync_WithAnnouncement_DropsAnnouncement()
    {
        Assert.Ignore("Test stub for IMaterialManagement.DropMaterialAnnouncementAsync(Guid, CancellationToken).");
    }

    [Test]
    public void RegisterContainerAsync_WithContainer_RegistersContainer()
    {
        Assert.Ignore("Test stub for IMaterialManagement.RegisterContainerAsync(IMaterialContainer, CancellationToken).");
    }

    [Test]
    public void DeregisterContainerAsync_WithContainer_DeregistersContainer()
    {
        Assert.Ignore("Test stub for IMaterialManagement.DeregisterContainerAsync(IMaterialContainer, CancellationToken).");
    }

    [Test]
    public void RecordLineageAsync_WithLineageEvent_RecordsLineageEvent()
    {
        Assert.Ignore("Test stub for IMaterialManagement.RecordLineageAsync(ILineageEvent, CancellationToken).");
    }

    [Test]
    public void GetLineage_WithContainerId_ReturnsLineageEventsForContainer()
    {
        Assert.Ignore("Test stub for IMaterialManagement.GetLineage(long).");
    }

    [Test]
    public void GetLineage_WithFilter_ReturnsMatchingLineageEvents()
    {
        Assert.Ignore("Test stub for IMaterialManagement.GetLineage(Func<ILineageEvent, bool>).");
    }

    private sealed class TestIdentity : IIdentity
    {
        public string Identifier { get; private set; } = string.Empty;

        public void SetIdentifier(string identifier) => Identifier = identifier;

        public bool Equals(IIdentity? other) =>
            other is not null && string.Equals(Identifier, other.Identifier, StringComparison.Ordinal);
    }
}
