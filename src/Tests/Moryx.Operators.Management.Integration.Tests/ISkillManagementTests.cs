// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Operators.Skills;

namespace Moryx.Operators.Management.Tests;

[TestFixture]
internal class ISkillManagementTests : TestsBase
{
    private ISkillManagement _facade;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _facade = _env.GetTestModule<ISkillManagement>();
        _env.GetTestModule<IOperatorManagement>().AddOperator(Operator);
    }

    [TearDown]
    public void TearDown() { 
        _env.StopTestModule();
    }


    [Test]
    public void Start_WithExpiredSkillInDB_FiltersSkill()
    {
        // Arrange
        _facade.CreateSkillType(SkillTypeCreationContext);
        _facade.CreateSkill(SkillCreationContext);
        _facade.CreateSkill(ExpiredSkillCreationContext);

        // Act
        _env.StopTestModule();
        _env.StartTestModule();

        // Assert
        Assert.That(ObjectsAreEqual(_facade.Skills.Single(), Skill));
    }

    #region Skill Type Tests

    [Test]
    public void CreateSkillType_WithCreationContext_CreatesSkillType()
    {
        // Arrange
        // Act
        var type = _facade.CreateSkillType(SkillTypeCreationContext);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ObjectsAreEqual(_facade.SkillTypes.Single(), SkillType));
            Assert.That(ObjectsAreEqual(type, SkillType));
        });
    }

    [Test]
    public void CreateSkillType_WithCreationContext_RaisesEvent()
    {
        // Arrange
        var eventArgs = new SkillTypeChangedEventArgs(SkillTypeChange.Deletion, UnknownSkillType);
        _facade.SkillTypeChanged += (sender, ev) => eventArgs = ev;

        // Act
        var type = _facade.CreateSkillType(SkillTypeCreationContext);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(eventArgs.Change, Is.EqualTo(SkillTypeChange.Creation));
            Assert.That(ObjectsAreEqual(eventArgs.SkillType, SkillType));
        });
    }

    [Test]
    public void UpdateSkillType_WithUpdatedType_UpdatesSkillType()
    {
        // Arrange
        var type = _facade.CreateSkillType(SkillTypeCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.UpdateSkillType(UpdatedSkillType));
            Assert.That(ObjectsAreEqual(_facade.SkillTypes.Single(), UpdatedSkillType));
        });
    }

    [Test]
    public void UpdateSkillType_WithUpdatedType_RaisesEvent()
    {
        // Arrange
        var eventArgs = new SkillTypeChangedEventArgs(SkillTypeChange.Deletion, UnknownSkillType);
        _facade.SkillTypeChanged += (sender, ev) => eventArgs = ev;
        var type = _facade.CreateSkillType(SkillTypeCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.UpdateSkillType(UpdatedSkillType));
            Assert.That(eventArgs.Change, Is.EqualTo(SkillTypeChange.Update));
            Assert.That(ObjectsAreEqual(eventArgs.SkillType, UpdatedSkillType));
        });
    }

    [Test]
    public void UpdateSkillType_WithUnknownType_ThrowsArgumentException()
    {
        // Arrange
        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => _facade.UpdateSkillType(UnknownSkillType));
            Assert.That(_facade.SkillTypes, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void DeleteSkillType_WithExistingId_DeletesSkillType()
    {
        // Arrange
        var type = _facade.CreateSkillType(SkillTypeCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.DeleteSkillType(SkillType.Id));
            Assert.That(_facade.SkillTypes, Is.Empty);
        });
    }

    [Test]
    public void DeleteSkillType_WithExistingId_RaisesEvent()
    {
        // Arrange
        var eventArgs = new SkillTypeChangedEventArgs(SkillTypeChange.Creation, UnknownSkillType);
        _facade.SkillTypeChanged += (sender, ev) => eventArgs = ev;
        var type = _facade.CreateSkillType(SkillTypeCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.DeleteSkillType(SkillType.Id));
            Assert.That(ObjectsAreEqual(eventArgs.SkillType, SkillType));
            Assert.That(eventArgs.Change, Is.EqualTo(SkillTypeChange.Deletion));
        });
    }

    [Test]
    public void DeleteSkillType_WithUnknownId_ThrowsArgumentException()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => _facade.DeleteSkillType(UnknownSkillType.Id));
    }

    [Test]
    public void DeleteSkillType_WithReferncingSkill_DeletesSkillTypeAndSkills()
    {
        // Arrange
        _facade.CreateSkillType(SkillTypeCreationContext);
        _facade.CreateSkill(SkillCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.DeleteSkillType(SkillType.Id));
            Assert.That(_facade.SkillTypes, Is.Empty);
            Assert.That(_facade.Skills, Is.Empty);
        });
    }

    [Test]
    public void DeleteSkillType_WithReferncingSkill_RaisesEvents()
    {
        // Arrange
        var typeEventArgs = new SkillTypeChangedEventArgs(SkillTypeChange.Creation, UnknownSkillType);
        _facade.SkillTypeChanged += (sender, ev) => typeEventArgs = ev;
        _facade.CreateSkillType(SkillTypeCreationContext);

        var skillEventArgs = new SkillChangedEventArgs(SkillChange.Creation, UnknownSkill);
        _facade.SkillChanged += (sender, ev) => skillEventArgs = ev;
        _facade.CreateSkill(SkillCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.DeleteSkillType(SkillType.Id));
            Assert.That(ObjectsAreEqual(typeEventArgs.SkillType, SkillType));
            Assert.That(typeEventArgs.Change, Is.EqualTo(SkillTypeChange.Deletion));
            Assert.That(ObjectsAreEqual(skillEventArgs.Skill, Skill));
            Assert.That(skillEventArgs.Change, Is.EqualTo(SkillChange.Deletion));
        });
    }

    #endregion

    #region Skill Tests

    [Test]
    public void CreateSkill_WithCreationContext_CreatesSkill()
    {
        // Arrange
        _facade.CreateSkillType(SkillTypeCreationContext);

        // Act
        var skill = _facade.CreateSkill(SkillCreationContext);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ObjectsAreEqual(_facade.Skills.Single(), Skill));
            Assert.That(ObjectsAreEqual(skill, Skill));
        });
    }

    [Test]
    public void CreateSkill_WithCreationContext_RaisesEvent()
    {
        // Arrange
        _facade.CreateSkillType(SkillTypeCreationContext);
        var eventArgs = new SkillChangedEventArgs(SkillChange.Deletion, UnknownSkill);
        _facade.SkillChanged += (sender, ev) => eventArgs = ev;

        // Act
        _facade.CreateSkill(SkillCreationContext);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(eventArgs.Change, Is.EqualTo(SkillChange.Creation));
            Assert.That(ObjectsAreEqual(eventArgs.Skill, Skill));
        });
    }

    [Test]
    public void DeleteSkill_WithExistingId_DeletesSkill()
    {
        // Arrange
        _facade.CreateSkillType(SkillTypeCreationContext);
        _facade.CreateSkill(SkillCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.DeleteSkill(Skill.Id));
            Assert.That(_facade.Skills, Is.Empty);
        });
    }

    [Test]
    public void DeleteSkill_WithExistingId_RaisesEvent()
    {
        // Arrange
        var eventArgs = new SkillChangedEventArgs(SkillChange.Creation, UnknownSkill);
        _facade.SkillChanged += (sender, ev) => eventArgs = ev;
        _facade.CreateSkillType(SkillTypeCreationContext);
        _facade.CreateSkill(SkillCreationContext);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => _facade.DeleteSkill(Skill.Id));
            Assert.That(ObjectsAreEqual(eventArgs.Skill, Skill));
            Assert.That(eventArgs.Change, Is.EqualTo(SkillChange.Deletion));
        });
    }

    [Test]
    public void DeleteSkill_WithUnknownId_ThrowsArgumentException()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => _facade.DeleteSkill(UnknownSkill.Id));
    }

    #endregion

}
