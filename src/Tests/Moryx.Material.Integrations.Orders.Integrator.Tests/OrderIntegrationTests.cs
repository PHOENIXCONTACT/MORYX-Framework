// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Material.Facade;
using Moryx.Material.Linking;
using Moryx.Material.States;
using Moryx.Modules;
using Moryx.Orders;
using Moryx.TestTools.IntegrationTest;
using Moryx.Tools;
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
    private IOrderIntegration _orderIntegration = null!; // ToDo: Remove null!

    [SetUp]
    public async Task SetUp()
    {
        ReflectionTool.TestMode = true;
        _containers.Clear();
        _operations.Clear();

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
            .ReturnsAsync((string orderNumber, string operationNumber, CancellationToken _) =>
                _operations.SingleOrDefault(o => o.Order.Number == orderNumber && o.Number == operationNumber));
    }

    private async Task SetupEnvironment()
    {
        var config = new IntegratorModuleConfig();
        config.Initialize();
        _env = new MoryxTestEnvironment(typeof(IntegratorModuleController), [_materialManagement, _orderManagement], config);

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
            Assert.That(container.LinkedOrder, Is.Not.Null);
            Assert.That(container.LinkedOrder, Is.Not.SameAs(originalReference));
            Assert.That(container.LinkedOrder!.OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(container.LinkedOrder.OperationNumber, Is.EqualTo(OperationNumber));
            Assert.That(container.LinkedOrder.State, Is.EqualTo(ReferenceState.Active));
            Assert.That(container.LinkedOrder.Status, Is.EqualTo(OperationStateClassification.Ready));
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
            Assert.That(container.LinkedOrder, Is.Not.Null);
            Assert.That(container.LinkedOrder, Is.Not.SameAs(originalReference));
            Assert.That(container.LinkedOrder!.OrderNumber, Is.EqualTo("UNKNOWN-ORDER"));
            Assert.That(container.LinkedOrder.OperationNumber, Is.EqualTo("9999"));
            Assert.That(container.LinkedOrder.State, Is.EqualTo(ReferenceState.Unavailable));
            Assert.That(container.LinkedOrder.Status, Is.Null);
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
            Assert.That(container.LinkedOrder, Is.Not.Null);
            Assert.That(container.LinkedOrder, Is.Not.SameAs(originalReference));
            Assert.That(container.LinkedOrder!.OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(container.LinkedOrder.OperationNumber, Is.EqualTo(OperationNumber));
            Assert.That(container.LinkedOrder.State, Is.EqualTo(ReferenceState.Active));
            Assert.That(container.LinkedOrder.Status, Is.EqualTo(OperationStateClassification.Running));
        });
    }

    [Test]
    public async Task RequestOrderLinkAsync_ForRegisteredContainer_AppliesActiveSynchronizedReferenceWithoutHandlingValidationErrors()
    {
        // Arrange
        var operation = CreateAndAddOperation(OrderNumber, OperationNumber, OperationStateClassification.Ready);
        RaiseOperationUpdatedToRunning(operation);

        var container = await CreateAndAddContainer();
        RaiseContainerStateChanged(container, new AvailableStateInformation());

        // Act
        await container.RequestOrderLinkAsync(OrderNumber, OperationNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(container.LinkedOrder, Is.Not.Null);
            Assert.That(container.LinkedOrder!.OrderNumber, Is.EqualTo(OrderNumber));
            Assert.That(container.LinkedOrder.OperationNumber, Is.EqualTo(OperationNumber));
            Assert.That(container.LinkedOrder.State, Is.EqualTo(ReferenceState.Active));
            Assert.That(container.LinkedOrder.Status, Is.EqualTo(OperationStateClassification.Running));
            Assert.That(container.ValidationErrorHandlingCount, Is.Zero);
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
            Assert.That(container.LinkedOrder!.State, Is.EqualTo(ReferenceState.Active));
            Assert.That(container.LinkedOrder.Status, Is.EqualTo(OperationStateClassification.Running));
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
            Assert.That(container.LinkedOrder!.State, Is.EqualTo(ReferenceState.Inactive));
            Assert.That(container.LinkedOrder.Status, Is.EqualTo(OperationStateClassification.Completed));
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
            LinkedOrder = linkedOrder
        };
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
            SetState(state);
        }

        public void SetState(OperationStateClassification state)
        {
            State = state;
            StateDisplayName = state.ToString();
        }
    }
}
