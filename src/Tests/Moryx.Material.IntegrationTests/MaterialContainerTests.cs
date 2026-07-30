// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;
using NUnit.Framework;

namespace Moryx.Material.IntegrationTests;

[TestFixture]
internal sealed class MaterialContainerTests : TestBase
{
    [Test]
    public async Task CreateUnsafe_OfBasicMaterialContainer_CreatesContainerResourceInUnitializedState()
    {
        // Arrange
        // Act
        var containerId = await _resourceManagement.CreateUnsafeAsync(typeof(BasicMaterialContainer), _ => Task.CompletedTask);

        // Assert
        var container = _resourceManagement.GetResource<IMaterialContainer>(containerId);
        Assert.Multiple(() =>
        {
            Assert.That(container, Is.Not.Null);
            Assert.That(container.Id, Is.GreaterThan(0));
            Assert.That(container.State, Is.EqualTo(StateClassification.Uninitialized));
        });
    }

    [Test]
    public async Task RegisterMaterial_FromHostResource_CreatesContainerInAvailableState()
    {
        // Arrange
        var hostId = await CreateContainerHostAsync();
        const string material = "Some Material";
        const double quantity = 42;
        const string unit = "Some Unit";

        // Act
        await _resourceManagement.ModifyUnsafeAsync(hostId, async resource =>
        {
            var host = (TestContainerHost)resource;
            await host.RegisterMaterialAsync(material, quantity, unit);
            return false;
        });

        // Assert
        var container = _resourceManagement.GetResource<IMaterialContainer>();
        Assert.Multiple(() =>
        {
            Assert.That(container, Is.Not.Null);
            Assert.That(container.Id, Is.GreaterThan(0));
            Assert.That(container.State, Is.EqualTo(StateClassification.Available));
            Assert.That(container.Material, Is.EqualTo(material));
            Assert.That(container.Quantity, Is.EqualTo(quantity));
            Assert.That(container.Unit, Is.EqualTo(unit));
        });
    }

    [Test]
    public async Task RequestMaterial_FromHostResource_CreatesContainerInRequestedState()
    {
        // Arrange
        var hostId = await CreateContainerHostAsync();
        var request = new MaterialRequest
        {
            Id = "Some Id",
            Material = "Some Material",
            RequestedQuantity = 42,
            Unit = "Some Unit",
            ExpectedArrival = DateTime.UtcNow,
        };

        // Act
        await _resourceManagement.ModifyUnsafeAsync(hostId, async resource =>
        {
            var host = (TestContainerHost)resource;
            await host.RequestMaterialAsync(request);
            return false;
        });

        // Assert
        var container = _resourceManagement.GetResource<IMaterialContainer>();
        Assert.Multiple(() =>
        {
            Assert.That(container, Is.Not.Null);
            Assert.That(container.Id, Is.GreaterThan(0));
            Assert.That(container.State, Is.EqualTo(StateClassification.Requested));
            Assert.That(container.Material, Is.EqualTo(request.Material));
            Assert.That(container.Quantity, Is.EqualTo(request.RequestedQuantity));
            Assert.That(container.Unit, Is.EqualTo(request.Unit));
        });
    }

    /// <summary>
    /// Test cases for the state persistence test, mapping each <see cref="StateInformation"/>
    /// subclass to the <see cref="StateClassification"/> that the <see cref="MaterialContainer"/>
    /// should be in after the resource management module was restarted.
    /// </summary>
    private static IEnumerable<TestCaseData> StatePersistenceCases()
    {
        yield return new TestCaseData(new RequestedStateInformation(), StateClassification.Requested)
            .SetName("State persistence: Requested");
        yield return new TestCaseData(new InboundStateInformation(), StateClassification.Inbound)
            .SetName("State persistence: Inbound");
        yield return new TestCaseData(new AvailableStateInformation(), StateClassification.Available)
            .SetName("State persistence: Available");
        yield return new TestCaseData(new OutboundStateInformation(), StateClassification.Outbound)
            .SetName("State persistence: Outbound");
        yield return new TestCaseData(new DeregisteredStateInformation(), StateClassification.Deregistered)
            .SetName("State persistence: Deregistered");
    }

    [TestCaseSource(nameof(StatePersistenceCases))]
    public async Task MaterialContainer_AfterRestart_RestoresStateFromStateInformation(
        StateInformation stateInformation, StateClassification expectedClassification)
    {
        // Arrange
        var containerId = await _resourceManagement.CreateUnsafeAsync(typeof(BasicMaterialContainer), resource =>
        {
            var container = (MaterialContainer)resource;
            container.StateInformation = stateInformation;
            return Task.CompletedTask;
        });

        // Act
        await RestartResourceManagementAsync();

        // Assert
        var container = _resourceManagement.GetResource<IMaterialContainer>(containerId);
        Assert.That(container, Is.Not.Null,
            "Container could not be retrieved after restarting the resource management.");
        Assert.That(container.State, Is.EqualTo(expectedClassification),
            "Container did not restore the expected state classification after restart.");
    }
}
