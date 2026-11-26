// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Linking;
using Moryx.Material.States;
using Moryx.Resources.Management;
using Moryx.TestTools.IntegrationTest;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Material.IntegrationTests;

[TestFixture]
internal sealed class MaterialContainerTests
{
    private MoryxTestEnvironment _env = null!;
    private IResourceManagement _resourceManagement = null!;

    [SetUp]
    public async Task SetUp()
    {
        ReflectionTool.TestMode = true;
        await SetupResourceManagement();
    }

    /// <summary>
    /// Setup the resource management module for integration tests of the save
    /// and reload behaviour of a material container
    /// </summary>
    private async Task SetupResourceManagement()
    {
        var config = new ModuleConfig();
        config.Initialize();
        _env = new MoryxTestEnvironment(typeof(ModuleController), [], config);

        await _env.StartTestModuleAsync();

        _resourceManagement = _env.GetTestModule<IResourceManagement>();
    }

    [TearDown]
    public Task TearDown()
    {
        return _env.StopTestModuleAsync();
    }

    private async Task RestartResourceManagementAsync()
    {
        await _env.StopTestModuleAsync();
        await _env.StartTestModuleAsync();
        _resourceManagement = _env.GetTestModule<IResourceManagement>();
    }

    /// <summary>
    /// Helper to create and persist a <see cref="TestContainerHost"/> through the
    /// resource management facade so that subsequent operations use the resource's
    /// <see cref="Resource.Graph"/> just like a real resource would.
    /// </summary>
    private async Task<long> CreateContainerHostAsync() =>
        await _resourceManagement.CreateUnsafeAsync(typeof(TestContainerHost), _ => Task.CompletedTask);

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
        var request = TestBase.DummyMaterialRequest;

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

    [Test]
    public async Task MaterialLink_RequestedInResourceConstructorWithoutIntegration_IsPendingAfterRestart()
    {
        // Arrange
        var containerId = await _resourceManagement.CreateUnsafeAsync(typeof(LinkedTestContainer), resource =>
        {
            // Resource constructors are invoked before the container is registered and do not await the request
            _ = ((LinkedTestContainer)resource).First.LinkAsync("A");
            return Task.CompletedTask;
        });

        // Act
        await RestartResourceManagementAsync();

        // Assert
        var link = _resourceManagement.ReadUnsafe(containerId, resource => ((LinkedTestContainer)resource).First);
        Assert.Multiple(() =>
        {
            Assert.That(link.HasPendingRequest, Is.True);
            Assert.That(link.PendingTarget!.Key, Is.EqualTo("A"));
            Assert.That(link.Reference, Is.Null);
        });
    }

    [Test]
    public async Task MaterialLink_WithAppliedManagedReference_RestoresDeclaredReferenceTypeAfterRestart()
    {
        // Arrange
        var containerId = await _resourceManagement.CreateUnsafeAsync(typeof(LinkedTestContainer), async resource =>
        {
            var container = (LinkedTestContainer)resource;
            await container.First.AttachAsync(container, new ManagingHandler());
            await container.First.LinkAsync("A");
        });

        // Act
        await RestartResourceManagementAsync();

        // Assert
        var (first, second, links) = _resourceManagement.ReadUnsafe(containerId, resource =>
        {
            var container = (LinkedTestContainer)resource;
            return (container.First, container.Second, container.Links);
        });
        Assert.Multiple(() =>
        {
            Assert.That(first.Reference, Is.TypeOf<TestReference>(),
                "The managed subtype of an integration must not leak into the persisted data");
            Assert.That(first.Reference!.Key, Is.EqualTo("A"));
            Assert.That(first.HasPendingRequest, Is.False);
            Assert.That(second.Reference, Is.Null);
            Assert.That(links, Is.EquivalentTo(new MaterialContainerLink[] { first, second }),
                "Restored links must be the instances discovered by the container.");
        });
    }

    private sealed class ManagingHandler : ILinkHandler<TestReference>
    {
        public Task<LinkingResponse<TestReference>> ValidateAsync(IMaterialContainer container, LinkingRequest<TestReference> request,
            CancellationToken cancellationToken)
            => Task.FromResult(new LinkingResponse<TestReference>(new ValidationContext(), new ManagedTestReference(request.Target!.Key)));

        public Task OnAppliedAsync(IMaterialContainer container, LinkingRequest<TestReference> request, LinkingResponse<TestReference> response,
            CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
