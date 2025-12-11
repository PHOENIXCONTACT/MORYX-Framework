// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Operators.Exceptions;

namespace Moryx.Operators.Management.Tests;

[TestFixture]
internal class IOperatorManagementTests : TestsBase
{
    private IOperatorManagement _facade;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _facade = _env.GetTestModule<IOperatorManagement>();
    }

    [TearDown]
    public void TearDown()
    {
        _env.StopTestModuleAsync();
    }

    [Test]
    public void AddOperator_WithNewOperator_AddsOperator()
    {
        // Arrange
        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.AddOperator(Operator));
            Assert.That(ObjectsAreEqual(_facade.Operators.Single(), Operator));
        });
    }

    [Test]
    public void AddOperator_WithNewOperator_RaisesChangeEvent()
    {
        // Arrange
        OperatorChangedEventArgs eventArgs = new(InvalidOperator);
        _facade.OperatorChanged += (sender, args) => eventArgs = args;

        // Act
        _facade.AddOperator(Operator);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(eventArgs.Change, Is.EqualTo(OperatorChange.Creation));
            Assert.That(ObjectsAreEqual(eventArgs.Operator, Operator));
        });
    }

    [Test]
    public void AddOperator_WithNewOperator_IsPersisted()
    {
        // Arrange
        // Act
        _facade.AddOperator(Operator);
        _env.StopTestModuleAsync();
        _env.StartTestModuleAsync();

        // Assert
        Assert.That(ObjectsAreEqual(_facade.Operators.Single(), Operator));
    }

    [Test]
    public void AddOperator_WithDuplicateOperator_ThrowsAlreadyExistsException()
    {
        // Arrange
        _facade.AddOperator(Operator);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.Throws<AlreadyExistsException>(() => _facade.AddOperator(Operator));
            Assert.That(_facade.Operators, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void AddOperator_WithInvalidIdentifier_ThrowsArgumentException()
    {
        // Arrange
        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => _facade.AddOperator(InvalidOperator));
            Assert.That(_facade.Operators, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void UpdateOperator_WithExisitingOperator_UpdatesOperator()
    {
        // Arrange
        _facade.AddOperator(Operator);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.UpdateOperator(UpdatedOperator));
            Assert.That(ObjectsAreEqual(_facade.Operators.Single(), UpdatedOperator));
        });
    }

    [Test]
    public void UpdateOperator_WithExisitingOperator_RaisesChangeEvent()
    {
        // Arrange
        OperatorChangedEventArgs eventArgs = new(InvalidOperator);
        _facade.OperatorChanged += (sender, args) => eventArgs = args;
        _facade.AddOperator(Operator);

        // Act
        _facade.UpdateOperator(UpdatedOperator);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(eventArgs.Change, Is.EqualTo(OperatorChange.Update));
            Assert.That(ObjectsAreEqual(eventArgs.Operator, UpdatedOperator));
        });
    }

    [Test]
    public void UpdateOperator_WithExisitingOperator_IsPersisted()
    {
        // Arrange
        _facade.AddOperator(Operator);

        // Act
        _facade.UpdateOperator(UpdatedOperator);
        _env.StopTestModuleAsync();
        _env.StartTestModuleAsync();

        // Assert
        Assert.That(ObjectsAreEqual(_facade.Operators.Single(), UpdatedOperator));
    }

    [Test]
    public void UpdateOperator_WithUnkownOperator_ThrowsArgumentException()
    {
        // Arrange
        _facade.AddOperator(Operator);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => _facade.UpdateOperator(UnknownOperator));
            Assert.That(ObjectsAreEqual(_facade.Operators.Single(), Operator));
        });
    }

    [Test]
    public void DeleteOperator_WithExisitingOperator_DeletesOperator()
    {
        // Arrange
        OperatorChangedEventArgs eventArgs = new(InvalidOperator);
        _facade.OperatorChanged += (sender, args) => eventArgs = args;
        _facade.AddOperator(Operator);

        // Act
        _facade.DeleteOperator(Operator.Identifier);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(eventArgs.Change, Is.EqualTo(OperatorChange.Deletion));
            Assert.That(ObjectsAreEqual(eventArgs.Operator, Operator));
        });
    }

    [Test]
    public void DeleteOperator_WithExisitingOperator_RaisesEvent()
    {
        // Arrange
        _facade.AddOperator(Operator);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.DeleteOperator(Operator.Identifier));
            Assert.That(_facade.Operators, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void DeleteOperator_WithExisitingOperator_IsPersisted()
    {
        // Arrange
        _facade.AddOperator(Operator);

        // Act
        _facade.DeleteOperator(Operator.Identifier);
        _env.StopTestModuleAsync();
        _env.StartTestModuleAsync();

        // Assert
        Assert.That(_facade.Operators, Has.Count.EqualTo(0));
    }

    [Test]
    public void DeleteOperator_WithUnkownOperator_ThrowsArgumentException()
    {
        // Arrange
        _facade.AddOperator(Operator);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => _facade.DeleteOperator(UnknownOperator.Identifier));
            Assert.That(ObjectsAreEqual(_facade.Operators.Single(), Operator));
        });
    }
}
