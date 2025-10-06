// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Management.Tests;

[TestFixture]
internal class IAttendanceManagementTests : TestsBase
{
    private IAttendanceManagement _facade;
    private readonly string _defaultOperatorIdentifier = "Default Operator Identifier";

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _facade = _env.GetTestModule<IAttendanceManagement>();
        _env.GetTestModule<IOperatorManagement>().AddOperator(AssignableOperator);
    }

    [TearDown]
    public void TearDown()
    {
        _env.StopTestModule();
    }

    [Test]
    public void Start_WithConfiguredDefaultOperator_CreatesDefaultOperator()
    {
        // Arrange
        // Act
        _config.DefaultOperator = _defaultOperatorIdentifier;
        _env.StopTestModule();
        _env.StartTestModule();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_facade.Operators, Has.Count.EqualTo(2));
            Assert.DoesNotThrow(() => _facade.Operators.Single(o => o.Identifier == _defaultOperatorIdentifier));
            Assert.That(_facade.DefaultOperator?.Identifier, Is.EqualTo(_defaultOperatorIdentifier));
        });
    }

    [Test]
    public void Start_WithExistingDefaultOperator_LeavesUnchanged()
    {
        // Arrange
        _config.DefaultOperator = _defaultOperatorIdentifier;
        _env.StopTestModule();
        _env.StartTestModule();

        // Act
        _env.StopTestModule();
        _env.StartTestModule();

        // Assert
        Assert.DoesNotThrow(() => _facade.Operators.Single(o => o.Identifier == _defaultOperatorIdentifier));
    }

    [Test]
    public void SignIn_WithUnassignedOperator_AssignsResource()
    {
        // Arrange
        // Act
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);

        // Assert
        var assignedOperator = _facade.Operators.Single();
        Assert.That(assignedOperator.AssignedResources, Has.Count.EqualTo(1));
    }

    [Test]
    public void SignIn_WithUnassignedOperator_SignsInUser()
    {
        // Arrange
        // Act
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);

        // Assert
        Assert.That(_facade.Operators.Single().SignedIn);
    }

    [Test]
    public void SignIn_WithUnassignedOperator_RaisesEvent()
    {
        // Arrange
        var eventArgs = InvalidAssignableOperator;
        _facade.OperatorSignedIn += (sender, op) => eventArgs = op;

        // Act
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);

        // Assert
        Assert.That(ObjectsAreEqual(AssignableOperator, eventArgs));
    }

    [Test]
    public void SignIn_WithAssignedOperator_DoesNothing()
    {
        // Arrange
        var eventArgs = InvalidAssignableOperator;
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);
        _facade.OperatorSignedIn += (sender, op) => eventArgs = op;

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.SignIn(AssignableOperator, FirstResourceMock.Object));
            Assert.That(ObjectsAreEqual(InvalidAssignableOperator, eventArgs));
        });
    }

    [Test]
    public void SignIn_OnAdditionalResource_AddsResourceAssignement()
    {
        // Arrange
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);

        // Act
        _facade.SignIn(AssignableOperator, SecondResourceMock.Object);

        // Assert
        var assignedOperator = _facade.Operators.Single();
        Assert.That(assignedOperator.AssignedResources, Has.Count.EqualTo(2));
    }

    [Test]
    public void SignIn_WithInvalidUser_ThrowsArgumentException()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => _facade.SignIn(InvalidAssignableOperator, FirstResourceMock.Object));
    }

    [Test]
    public void SignOut_WithAssignedOperator_UnassignsResource()
    {
        // Arrange
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);

        // Act
        _facade.SignOut(AssignableOperator, FirstResourceMock.Object);

        // Assert
        Assert.That(_facade.Operators.Single().AssignedResources, Has.Count.EqualTo(0));
    }

    [Test]
    public void SignOut_WithAssignedOperator_SignsOutUser()
    {
        // Arrange
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);

        // Act
        _facade.SignOut(AssignableOperator, FirstResourceMock.Object);

        // Assert
        Assert.That(_facade.Operators.Single().SignedIn, Is.False);
    }

    [Test]
    public void SignOut_WithAssignedOperator_RaisesEvent()
    {
        // Arrange
        var eventArgs = InvalidAssignableOperator;
        _facade.OperatorSignedOut += (sender, op) => eventArgs = op;
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);

        // Act
        _facade.SignOut(AssignableOperator, FirstResourceMock.Object);

        // Assert
        Assert.That(ObjectsAreEqual(AssignableOperator, eventArgs));
    }

    [Test]
    public void SignOut_OfUserWithMultipleAssignements_UnassignsOnlyOneResource()
    {
        // Arrange
        _facade.SignIn(AssignableOperator, FirstResourceMock.Object);
        _facade.SignIn(AssignableOperator, SecondResourceMock.Object);

        // Act
        _facade.SignOut(AssignableOperator, FirstResourceMock.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_facade.Operators.Single().AssignedResources, Has.Count.EqualTo(1));
            Assert.That(_facade.Operators.Single().AssignedResources.Single().Id, Is.EqualTo(SecondResourceMock.Object.Id));
        });
    }

    [Test]
    public void SignOut_WithUnassignedOperator_DoesNothing()
    {
        // Arrange
        var eventArgs = InvalidAssignableOperator;
        _facade.OperatorSignedOut += (sender, op) => eventArgs = op;

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.SignOut(AssignableOperator, FirstResourceMock.Object));
            Assert.That(ObjectsAreEqual(InvalidAssignableOperator, eventArgs));
        });
    }
}
