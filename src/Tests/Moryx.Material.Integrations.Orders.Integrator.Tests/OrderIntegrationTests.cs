// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Material.Facade;
using Moryx.Material.Linking;
using Moryx.Material.States;
using Moryx.Modules;
using Moryx.Orders;
using Moryx.Serialization;
using Moryx.TestTools.IntegrationTest;
using Moryx.Tools;
using Newtonsoft.Json;
using NUnit.Framework;
using IntegratorModuleConfig = Moryx.Material.Integrations.Orders.Integrator.ModuleConfig;
using IntegratorModuleController = Moryx.Material.Integrations.Orders.Integrator.ModuleController;
using Operation = Moryx.Orders.Operation;

namespace Moryx.Material.Integrations.Orders.Integrator.Tests;

// ToDo: While this is a seperate module, does it make sense to split the tests in a seperate project as well?
// I think having the integration tests under the Moryx.Material.IntegrationTests project for the root namespace
// would be a little easier to understand for later developers.
[TestFixture]
internal sealed class OrderIntegrationTests
{
    private const string OrderNumber = "ORDER-1";
    private const string OperationNumber = "0010";

    private readonly List<IMaterialContainer> _containers = [];
    private readonly List<Operation> _operations = [];

    private Mock<IMaterialManagement> _materialManagement = null!;
    private Mock<IOrderManagement> _orderManagement = null!;
    private MoryxTestEnvironment _env = null!;
    private IntegratorModuleConfig _config = null!;
    private IOrderIntegration _orderIntegration = null!; // ToDo: Remove null!

    [SetUp]
    public async Task SetUp()
    {
        ReflectionTool.TestMode = true;
        _containers.Clear();
        _operations.Clear();
        BlockingOrderLinkingHook.RequestCount = 0;
        BlockingOrderLinkingHook.AppliedCount = 0;

        SetupMaterialManagementMock();
        SetupOrderManagementMock();
        await SetupEnvironment();

        _orderIntegration = _env.GetTestModule<IOrderIntegration>();
    }

    private void SetupMaterialManagementMock()
    {
        _materialManagement = MoryxTestEnvironment.CreateModuleMock<IMaterialManagement>();
        _materialManagement.Setup(m => m.GetContainers()).Returns([.. _containers]);
        _materialManagement.Setup(m => m.GetContainers(It.IsAny<Func<IMaterialContainer, bool>>()))
            .Returns((Func<IMaterialContainer, bool> filter) => [.. _containers.Where(filter)]);
    }

    private void SetupOrderManagementMock()
    {
        _orderManagement = MoryxTestEnvironment.CreateModuleMock<IOrderManagement>();
        _orderManagement.Setup(m => m.GetOperations(It.IsAny<Func<Operation, bool>>()))
            .Returns((Func<Operation, bool> filter) => [.. _operations.Where(filter)]);
        _orderManagement.Setup(m => m.LoadOperationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns((string orderNumber, string operationNumber, CancellationToken _) => LoadOperationAsync(orderNumber, operationNumber));
    }

    // Completes asynchronously like a real facade to cover responses arriving after the request was raised
    private async Task<Operation?> LoadOperationAsync(string orderNumber, string? operationNumber)
    {
        await Task.Delay(5);
        return _operations.SingleOrDefault(o => o.Order.Number == orderNumber && o.Number == operationNumber);
    }

    private async Task SetupEnvironment()
    {
        _config = new IntegratorModuleConfig();
        _config.Initialize();
        _env = new MoryxTestEnvironment(typeof(IntegratorModuleController), [_materialManagement, _orderManagement], _config);

        await _env.StartTestModuleAsync();
    }

    [TearDown]
    public Task TearDown() => _env.StopTestModuleAsync();

    [Test]
    public async Task Start_WithExistingOrderLinkedContainer_SubstitutesReferenceWithActiveSynchronizedReference()
    {
        // Arrange
        var originalReference = new OrderReference(OrderNumber, OperationNumber);
        var container = await CreateAndAddContainer(originalReference);
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);

        // Act
        await RestartModuleAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(container.OrderLink.Reference, Is.Not.Null);
            Assert.That(container.OrderLink.Reference, Is.Not.SameAs(originalReference));
            Assert.That(container.OrderLink.Reference!.OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(container.OrderLink.Reference.OperationNumber, Is.EqualTo(OperationNumber));
            Assert.That(container.OrderLink.Reference.State, Is.EqualTo(ReferenceState.Active));
            Assert.That(container.OrderLink.Reference.Status, Is.EqualTo(OperationStateClassification.Ready));
            Assert.That(container.OrderLink.Reference.Source, Is.TypeOf<TestOperationSource>());
        });
    }

    [Test]
    public async Task Start_WithExistingContainerLinkedToUnavailableOperation_SubstitutesReferenceWithUnavailableReference()
    {
        // Arrange
        var originalReference = new OrderReference("UNKNOWN-ORDER", "9999");
        var container = await CreateAndAddContainer(originalReference);
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);

        // Act
        await RestartModuleAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(container.OrderLink.Reference, Is.Not.Null);
            Assert.That(container.OrderLink.Reference, Is.Not.SameAs(originalReference));
            Assert.That(container.OrderLink.Reference!.OrderNumber, Is.EqualTo("UNKNOWN-ORDER"));
            Assert.That(container.OrderLink.Reference.OperationNumber, Is.EqualTo("9999"));
            Assert.That(container.OrderLink.Reference.State, Is.EqualTo(ReferenceState.Unavailable));
            Assert.That(container.OrderLink.Reference.Status, Is.Null);
            Assert.That(container.OrderLink.Reference.Source, Is.Null);
        });
    }

    [Test]
    public async Task ContainerStateChanged_WithNewOrderLinkedContainer_SubstitutesReferenceWithActiveSynchronizedReferenceAsync()
    {
        // Arrange
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Running);
        RaiseOperationUpdatedToRunning(operation);

        var originalReference = new OrderReference(OrderNumber, OperationNumber);
        var container = await CreateAndAddContainer(originalReference);

        // Act
        RaiseContainerStateChanged(container, new AvailableStateInformation());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(container.OrderLink.Reference, Is.Not.Null);
            Assert.That(container.OrderLink.Reference, Is.Not.SameAs(originalReference));
            Assert.That(container.OrderLink.Reference!.OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(container.OrderLink.Reference.OperationNumber, Is.EqualTo(OperationNumber));
            Assert.That(container.OrderLink.Reference.Status, Is.EqualTo(OperationStateClassification.Running));
            Assert.That(container.OrderLink.Reference.Source, Is.TypeOf<TestOperationSource>());
            Assert.That(container.OrderLink.Reference.State, Is.EqualTo(ReferenceState.Active));
        });
    }

    [Test]
    public async Task LinkAsync_ForRegisteredContainer_AppliesActiveSynchronizedReference()
    {
        // Arrange
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);
        RaiseOperationUpdatedToRunning(operation);

        var container = await CreateAndAddContainer();
        RaiseContainerStateChanged(container, new AvailableStateInformation());

        // Act
        var result = await container.OrderLink.LinkAsync(OrderNumber, OperationNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Applied));
            Assert.That(container.OrderLink.Reference, Is.Not.Null);
            Assert.That(container.OrderLink.Reference!.OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(container.OrderLink.Reference.OperationNumber, Is.EqualTo(OperationNumber));
            Assert.That(container.OrderLink.Reference.State, Is.EqualTo(ReferenceState.Active));
            Assert.That(container.OrderLink.Reference.Status, Is.EqualTo(OperationStateClassification.Running));
            Assert.That(container.OrderLink.Reference.Source, Is.TypeOf<TestOperationSource>());
        });
    }

    // TODO: This behaviour should be coming from a hook, optionally we would definitely want unknown orders to be linkable
    // to not limit use cases which register material before the order is known.
    [Test]
    public async Task LinkAsync_ForUnknownOrder_IsRejectedAndKeepsContainerUnlinked()
    {
        // Arrange
        var container = await CreateAndAddContainer();
        RaiseContainerStateChanged(container, new AvailableStateInformation());

        // Act
        var result = await container.OrderLink.LinkAsync("UNKNOWN-ORDER", "9999");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Rejected));
            Assert.That(result.Context.HasErrors, Is.True);
            Assert.That(container.OrderLink.Reference, Is.Null);
        });
    }

    [Test]
    public async Task LinkAsync_WithBlockingHook_IsRejectedWithoutRunningAppliedPhase()
    {
        // Arrange
        CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Running);
        _config.Hooks.Add(new OrderLinkingHookConfig { PluginName = BlockingOrderLinkingHook.PluginName });

        var container = await CreateAndAddContainer();
        RaiseContainerStateChanged(container, new AvailableStateInformation());

        // Act
        var result = await container.OrderLink.LinkAsync(OrderNumber, OperationNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(LinkingStatus.Rejected));
            Assert.That(result.Context.Entries.Single().HookType, Is.EqualTo(typeof(BlockingOrderLinkingHook)));
            Assert.That(BlockingOrderLinkingHook.RequestCount, Is.EqualTo(1));
            Assert.That(BlockingOrderLinkingHook.AppliedCount, Is.Zero);
            Assert.That(container.OrderLink.Reference, Is.Null);
        });
    }

    [Test]
    public async Task LinkAsync_BeforeContainerIsRegistered_IsProcessedOnceContainerIsRegistered()
    {
        // Arrange
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);
        RaiseOperationUpdatedToRunning(operation);
        var container = await CreateAndAddContainer();
        var applied = new TaskCompletionSource<LinkAppliedEventArgs<OrderReference>>(TaskCreationOptions.RunContinuationsAsynchronously);
        container.OrderLink.Applied += (_, e) => applied.TrySetResult(e);

        // Act
        // Like a resource constructor, the link is requested before the integration knows the container
        var pending = await container.OrderLink.LinkAsync(OrderNumber, OperationNumber);
        RaiseContainerStateChanged(container, new AvailableStateInformation());

        // Assert
        var appliedArgs = await applied.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Multiple(() =>
        {
            Assert.That(pending.Status, Is.EqualTo(LinkingStatus.Pending));
            Assert.That(appliedArgs.Container, Is.SameAs(container));
            Assert.That(container.OrderLink.HasPendingRequest, Is.False);
            Assert.That(container.OrderLink.Reference!.OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(container.OrderLink.Reference.State, Is.EqualTo(ReferenceState.Active));
        });
    }

    // TODO: Remove dependency to products here, add a dummy link instead
    [Test]
    public async Task LinkAsync_OfContainerLinkedToOrderAndProduct_OnlyAppliesOrderLink()
    {
        // Arrange
        CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Running);
        var container = new MockOrderAndProductLinkedMaterialContainer
        {
            Id = Random.Shared.Next(1, int.MaxValue),
            Name = "Combined Container",
            StateInformation = new AvailableStateInformation(),
        };
        await ((IAsyncInitializablePlugin)container).InitializeAsync();
        _containers.Add(container);
        RaiseContainerStateChanged(container, new AvailableStateInformation());

        // Act
        var orderResult = await container.OrderLink.LinkAsync(OrderNumber, OperationNumber);
        var productResult = await container.ProductLink.LinkAsync("PRD01-01");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(container.Links, Has.Count.EqualTo(2));
            Assert.That(orderResult.Status, Is.EqualTo(LinkingStatus.Applied));
            Assert.That(container.OrderLink.Reference!.OrderNumber, Is.EqualTo(OrderNumber));
            // No product integration is running in this environment
            Assert.That(productResult.Status, Is.EqualTo(LinkingStatus.Pending));
            Assert.That(container.ProductLink.PendingTarget!.ProductIdentity, Is.EqualTo("PRD01-01"));
        });
    }

    [Test]
    public async Task OperationUpdated_ForSynchronizedContainer_UpdatesReferenceStatus()
    {
        // Arrange
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);
        var container = await CreateAndAddContainer(new OrderReference(OrderNumber, OperationNumber));
        await RestartModuleAsync();

        // Act
        RaiseOperationUpdatedToRunning(operation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(container.OrderLink.Reference!.State, Is.EqualTo(ReferenceState.Active));
            Assert.That(container.OrderLink.Reference.Status, Is.EqualTo(OperationStateClassification.Running));
        });
    }

    [Test]
    public async Task OperationCompleted_ForSynchronizedContainer_UpdatesReferenceStatusAndMarksReferenceInactive()
    {
        // Arrange
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);
        var container = await CreateAndAddContainer(new OrderReference(OrderNumber, OperationNumber));
        await RestartModuleAsync();

        // Act
        RaiseOperationCompleted(operation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(container.OrderLink.Reference!.State, Is.EqualTo(ReferenceState.Inactive));
            Assert.That(container.OrderLink.Reference.Status, Is.EqualTo(OperationStateClassification.Completed));
        });
    }

    [Test]
    public async Task GetOrderReferences_WithRunningOperation_ReturnsReferences()
    {
        // Arrange
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);
        RaiseOperationUpdatedToRunning(operation);

        // Act
        var references = _orderIntegration.GetOrderReferences();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(references, Has.Count.EqualTo(1));
            Assert.That(references.Single().OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(references.Single().OperationNumber, Is.EqualTo(OperationNumber));
        });
    }

    private async Task RestartModuleAsync()
    {
        await _env.StopTestModuleAsync();
        await _env.StartTestModuleAsync();
    }

    private async Task<MockOrderLinkedMaterialContainer> CreateAndAddContainer(OrderReference? linkedOrder = null)
    {
        var container = new MockOrderLinkedMaterialContainer
        {
            Id = Random.Shared.Next(1, int.MaxValue),
            Name = "Test Container",
            StateInformation = new AvailableStateInformation(),
        };
        // TODO: Split this in a seperate test on the container:
        // - The MockContainer should just initialize the link on start
        // - An integration test case for the container should generally validate whether links and subclasses of links are correctly serialized
        if (linkedOrder is not null)
        {
            // Restore the link like the resource management does when loading a persisted container
            var extensionData = JsonConvert.SerializeObject(new { OrderLink = new { Reference = linkedOrder } }, JsonSettings.Minimal);
            JsonConvert.PopulateObject(extensionData, container, JsonSettings.Minimal);
        }
        await ((IAsyncInitializablePlugin)container).InitializeAsync();
        await ((IAsyncInitializablePlugin)container).StartAsync();
        _containers.Add(container);
        return container;
    }

    private MockOperation CreateAndAddOperation(string orderNumber, string operationNumber, OperationStateClassification state)
    {
        var order = new MockOrder(orderNumber);
        var operation = new MockOperation(order, operationNumber, state);
        _operations.Add(operation);
        return operation;
    }

    private void RaiseContainerStateChanged(IMaterialContainer container, StateInformation newState)
    {
        _materialManagement.Raise(m => m.ContainerStateChanged += null, new ContainerStateChangedEventArgs(container, null, newState));
    }

    private void RaiseOperationUpdatedToRunning(Operation operation)
    {
        ((MockOperation)operation).SetState(OperationStateClassification.Running);
        _orderManagement.Raise(m => m.OperationUpdated += null, new OperationChangedEventArgs(operation));
    }

    private void RaiseOperationCompleted(Operation operation)
    {
        ((MockOperation)operation).SetState(OperationStateClassification.Completed);
        _orderManagement.Raise(m => m.OperationCompleted += null!,
            new OperationReportEventArgs(operation, new OperationReport(ConfirmationType.Final, 1, 0, null!)));
    }

    private sealed class MockOrder : Order
    {
        public MockOrder(string number)
        {
            Number = number;
            Type = "Test";
            Operations = [];
        }
    }

    private sealed class MockOperation : Operation
    {
        public MockOperation(Order order, string number, OperationStateClassification state)
        {
            Identifier = Guid.NewGuid();
            Order = order;
            Number = number;
            Name = number;
            Source = new TestOperationSource();
            SetState(state);
        }

        public void SetState(OperationStateClassification state)
        {
            State = state;
            StateDisplayName = state.ToString();
        }
    }

    private sealed class TestOperationSource : IOperationSource
    {
        public string Type => nameof(TestOperationSource);
    }
}
