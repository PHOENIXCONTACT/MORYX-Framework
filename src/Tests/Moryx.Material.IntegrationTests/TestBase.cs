// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material;
using Moryx.Material.Facade;
using Moryx.Material.Management;
using Moryx.Material.States;
using Moryx.Modules;
using Moryx.TestTools.IntegrationTest;
using Moryx.Tools;
using NUnit.Framework;


namespace Moryx.Material.IntegrationTests;

[TestFixture]
internal abstract class TestBase
{
    protected MoryxTestEnvironment _env = null!;
    protected Mock<IResourceManagement> _resourceManagementMock = null!;
    protected Mock<IResourceTypeTree> _resourceTypeTreeMock = null!;

    protected IMaterialManagement _materialManagement = null!;

    [SetUp]
    public virtual async Task SetUp()
    {
        ReflectionTool.TestMode = true;
        await SetupMaterialManagementAsync();
    }

    /// <summary>
    /// Setup the material management module for integration tests, e.g. for the facade
    /// </summary>
    private async Task SetupMaterialManagementAsync()
    {
        var config = new ModuleConfig();
        config.Initialize();

        CreateResourceManagementMock();
        CreateResourceTypeTreeMock();

        _env = new MoryxTestEnvironment(typeof(ModuleController), [_resourceManagementMock, _resourceTypeTreeMock], config);
        await _env.StartTestModuleAsync();
        _materialManagement = _env.GetTestModule<IMaterialManagement>();
    }

    private void CreateResourceTypeTreeMock()
    {
        _resourceTypeTreeMock = MoryxTestEnvironment.CreateModuleMock<IResourceTypeTree>();
        _resourceTypeTreeMock.Setup(t => t.SupportedTypes(It.Is<Type>(t => t == typeof(IMaterialContainer))))
            .Returns((Type constraint) => [new ResourceTypeNodeMock(nameof(BasicMaterialContainer), typeof(BasicMaterialContainer), true),
                new ResourceTypeNodeMock(nameof(MaterialContainer), typeof(MaterialContainer), false)]);
    }

    private void CreateResourceManagementMock()
    {
        _resourceManagementMock = MoryxTestEnvironment.CreateModuleMock<IResourceManagement>();

        // Ensure pool startup does not fail when querying existing containers
        _resourceManagementMock.Setup(r => r.GetResources<IMaterialContainer>())
            .Returns([]);

        // Setup CreateUnsafeAsync to return a fixed id for any IMaterialContainer creation
        // and raise ResourceAdded with a dummy container configured from DummyMaterialRequest
        _resourceManagementMock.Setup(r => r.CreateUnsafeAsync(It.Is<Type>(t => typeof(IMaterialContainer).IsAssignableFrom(t)),
                It.IsAny<Func<Resource, Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Type, Func<Resource, Task>, CancellationToken>(async (type, initializer, ct) =>
            {
                var container = new MockMaterialContainer() { Id = 42L };
                ((IAsyncInitializablePlugin)container).InitializeAsync().GetAwaiter().GetResult();
                ((IAsyncInitializablePlugin)container).StartAsync().GetAwaiter().GetResult();
                // Run initializer to mimic RM behavior (may adjust name/material etc.)
                initializer?.Invoke(container).GetAwaiter().GetResult();
                // Announce container addition so ContainerPool can track it
                _resourceManagementMock.Raise(m => m.ResourceAdded += null, _resourceManagementMock.Object, container);
            })
            .ReturnsAsync(42L);
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        await _env.StopTestModuleAsync();
    }

    public static DateTimeOffset CreationTime => new(1923, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static DateTimeOffset ArrivalTime => new(1923, 1, 1, 1, 1, 1, TimeSpan.Zero);

    public static MockIdentifier DummyIdentifier = new() { Identifier = "Dummy Material Container" };

    public static MaterialRequest DummyMaterialRequest { get; } = new()
    {
        Id = "Some Id",
        ContainerIdentity = DummyIdentifier,
        Material = "Some Material",
        RequestedQuantity = 1337,
        Unit = "Pcs.",
        ExpectedArrival = ArrivalTime,
        CreatedAt = CreationTime
    };

    /// <summary>
    /// Creates a MockMaterialContainer prepopulated from DummyMaterialRequest
    /// </summary>
    protected static MockMaterialContainer CreateRequestedDummyMaterialContainer(long id) => new()
    {
        Id = id,
        Identity = DummyMaterialRequest.ContainerIdentity,
        Name = $"Request-{DummyMaterialRequest.Id}",
        Material = DummyMaterialRequest.Material,
        Quantity = DummyMaterialRequest.RequestedQuantity,
        Unit = DummyMaterialRequest.Unit,
        StateInformation = new RequestedStateInformation
        {
            RequestId = DummyMaterialRequest.Id,
            ExpectedArrival = DummyMaterialRequest.ExpectedArrival
        }
    };

    /// <summary>
    /// Creates a <see cref="MockMaterialContainer"/> prepopulated in the available state
    /// </summary>
    protected static MockMaterialContainer CreateAvailableDummyMaterialContainer(long id)
    {
        var container = new MockMaterialContainer()
        {
            Id = id,
            Identity = DummyIdentifier,
            Name = "Available Material Container",
            Material = DummyMaterialRequest.Material,
            Quantity = DummyMaterialRequest.RequestedQuantity,
            Unit = DummyMaterialRequest.Unit,
            StateInformation = new AvailableStateInformation(),
        };

        ((IAsyncInitializablePlugin)container).InitializeAsync().GetAwaiter().GetResult();
        ((IAsyncInitializablePlugin)container).StartAsync().GetAwaiter().GetResult();

        return container;
    }
}
