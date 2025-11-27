// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Operators.Skills;
using Moryx.Runtime.Modules;

namespace Moryx.Operators.Management.Tests;

[TestFixture]
internal class ModuleTests : TestsBase
{
    private IOperatorManagement _operatorsFacade;
    private ISkillManagement _skillsFacade;
    private IAttendanceManagement _attendingFacade;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _operatorsFacade = _env.GetTestModule<IOperatorManagement>();
        _skillsFacade = _env.GetTestModule<ISkillManagement>();
        _attendingFacade = _env.GetTestModule<IAttendanceManagement>();
    }

    [Test]
    public async Task Start_WhenModuleIsStopped_StartsModule()
    {
        // Arrange
        await _env.StopTestModuleAsync();

        // Act
        var module = await _env.StartTestModuleAsync();

        // Assert
        Assert.That(module.State, Is.EqualTo(ServerModuleState.Running));
    }

    [Test]
    public async Task Stop_WhenModuleIsRunning_StopsModule()
    {
        // Arrange
        // Act
        var module = await _env.StopTestModuleAsync();

        // Assert
        Assert.That(module.State, Is.EqualTo(ServerModuleState.Stopped));
    }

    [Test]
    public async Task Start_WhenDatabaseIsFilled_StartsModule()
    {
        // Arrange
        _operatorsFacade.AddOperator(Operator);
        _operatorsFacade.AddOperator(AssignableOperator);
        _attendingFacade.SignIn(AssignableOperator, FirstResourceMock.Object);
        _skillsFacade.CreateSkillType(SkillTypeCreationContext);
        _skillsFacade.CreateSkill(SkillCreationContext);

        // Act
        await _env.StopTestModuleAsync();
        var module = await _env.StartTestModuleAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(module.State, Is.EqualTo(ServerModuleState.Running),
                "Module is not in state running");
            Assert.That(_operatorsFacade.Operators, Has.Count.EqualTo(2),
                "Operators do not match after restart");
            Assert.DoesNotThrow(() => _attendingFacade.Operators.Single(o => o.AssignedResources.SingleOrDefault()?.Id == FirstResourceMock.Object.Id),
                "Attended resources do not match after restart");
            Assert.That(ObjectsAreEqual(_skillsFacade.SkillTypes.Single(), SkillType),
                "Skill types do not match after restart");
            Assert.That(ObjectsAreEqual(_skillsFacade.Skills.Single(), Skill),
                "Skills do not match after restart");
        });
    }

    [Test]
    public async Task AnyMethod_WhenFacadeNotActivated_ThrowsHealthStateException()
    {
        // Arrange
        await _env.StopTestModuleAsync();

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(() => _operatorsFacade.Operators, Throws.TypeOf<HealthStateException>());
            Assert.That(() => _operatorsFacade.AddOperator(Operator), Throws.TypeOf<HealthStateException>());
            Assert.That(() => _operatorsFacade.UpdateOperator(Operator), Throws.TypeOf<HealthStateException>());
            Assert.That(() => _operatorsFacade.DeleteOperator(Operator.Identifier), Throws.TypeOf<HealthStateException>());

            Assert.That(() => _attendingFacade.DefaultOperator, Throws.TypeOf<HealthStateException>());
            Assert.That(() => _attendingFacade.SignIn(AssignableOperator, FirstResourceMock.Object), Throws.TypeOf<HealthStateException>());
            Assert.That(() => _attendingFacade.SignOut(AssignableOperator, FirstResourceMock.Object), Throws.TypeOf<HealthStateException>());

            Assert.That(() => _skillsFacade.SkillTypes, Throws.TypeOf<HealthStateException>());
            Assert.That(() => _skillsFacade.Skills, Throws.TypeOf<HealthStateException>());
            Assert.That(() => _skillsFacade.CreateSkillType(SkillTypeCreationContext), Throws.TypeOf<HealthStateException>());
            Assert.That(() => _skillsFacade.UpdateSkillType(UpdatedSkillType), Throws.TypeOf<HealthStateException>());
            Assert.That(() => _skillsFacade.DeleteSkillType(SkillType.Id), Throws.TypeOf<HealthStateException>());
            Assert.That(() => _skillsFacade.CreateSkill(SkillCreationContext), Throws.TypeOf<HealthStateException>());
            Assert.That(() => _skillsFacade.DeleteSkill(Skill.Id), Throws.TypeOf<HealthStateException>());
        });
    }
}
