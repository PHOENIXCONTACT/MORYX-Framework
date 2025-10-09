// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Skills;
using Moryx.TestTools.IntegrationTest;
using Moryx.Tools;

namespace Moryx.Operators.Management.Tests;

[TestFixture]
internal abstract class TestsBase
{
    protected ModuleConfig _config = new();
    protected Mock<ResourceManagement> _resourceManagementMock;
    protected MoryxTestEnvironment _env;

    [SetUp]
    public virtual void SetUp()
    {
        ReflectionTool.TestMode = true;
        _resourceManagementMock = MoryxTestEnvironment.CreateModuleMock<ResourceManagement>();
        _resourceManagementMock.Setup(r => r.GetResources<IOperatorAssignable>()).Returns([FirstResourceMock.Object, SecondResourceMock.Object]);
        _env = new MoryxTestEnvironment(typeof(ModuleController), [_resourceManagementMock], _config);
        _env.StartTestModule();
    }

    #region Operator Test Tools

    /// <summary>
    /// Standard test operator
    /// </summary>
    protected static Operator Operator
        => CreateOperator("Test Operator", "Test First Name", "Test Last Name", "Test Pseudonym");

    /// <summary>
    /// Updated test operator with changed properties but identical identifier
    /// </summary>
    protected static Operator UpdatedOperator
        => CreateOperator("Test Operator", "Updated First Name", "Updated Last Name", "Updated Pseudonym");

    /// <summary>
    /// A test operator which should not be added to the database (otherwise it wouldn't be unknown anymore...)
    /// </summary>
    protected static Operator UnknownOperator => CreateOperator("Not " + Operator.Identifier, "-", "-", "-");

    /// <summary>
    /// A test operator with an invalid identifier
    /// </summary>
    protected static Operator InvalidOperator => CreateOperator("", "", "", "");

    /// <summary>
    /// Creates an operator with the given parameters
    /// </summary>
    private static Operator CreateOperator(string identifier, string firstName, string lastName, string pseudonym = "")
        => new(identifier) { FirstName = firstName, LastName = lastName, Pseudonym = pseudonym };

    /// <summary>
    /// Verifies whether all properties of the two operators are equal, i.e. checks for value based equality.
    /// </summary>
    protected static bool ObjectsAreEqual(Operator o1, Operator o2)
        => o1.Identifier == o2.Identifier && o1.FirstName == o2.FirstName && o1.LastName == o2.LastName && o1.Pseudonym == o2.Pseudonym;

    #endregion

    #region Attendance Test Tools

    /// <summary>
    /// An assignable test operator
    /// </summary>
    protected static AssignableOperator AssignableOperator
        => CreateAssignableOperator("Assignable Test Operator", "Assignable First Name", "Assignable Last Name", "Assignable Pseudonym");

    /// <summary>
    /// An invalid assignable test operator
    /// </summary>
    protected static AssignableOperator InvalidAssignableOperator => CreateAssignableOperator("", "", "", "");

    /// <summary>
    /// Mock of an IOperatorAssignable resource with ID 1
    /// </summary>
    protected static readonly Mock<IOperatorAssignable> FirstResourceMock = CreateResourceMock(1);

    /// <summary>
    /// Mock of an IOperatorAssignable resource with ID 1
    /// </summary>
    protected static readonly Mock<IOperatorAssignable> SecondResourceMock = CreateResourceMock(2);

    /// <summary>
    /// Creates an IOperatorAssignable resource mock with the given id
    /// </summary>
    private static Mock<IOperatorAssignable> CreateResourceMock(int id)
    {
        var mock = new Mock<IOperatorAssignable>();
        mock.SetupGet(r => r.Id).Returns(id);
        return mock;
    }

    /// <summary>
    /// Creates an assignable operator with the given parameters and no assigned resources
    /// </summary>
    private static AssignableOperator CreateAssignableOperator(string identifier, string firstName, string lastName, string pseudonym = "")
        => new(identifier) { FirstName = firstName, LastName = lastName, Pseudonym = pseudonym };

    /// <summary>
    /// Verifies whether all properties of the two assignable operators are equal, i.e. checks for value based equality.
    /// </summary>
    protected static bool ObjectsAreEqual(AssignableOperator o1, AssignableOperator o2)
        => o1.Identifier == o2.Identifier && o1.FirstName == o2.FirstName && o1.LastName == o2.LastName &&
        o1.Pseudonym == o2.Pseudonym && ObjectsAreEqual(o1.AssignedResources, o2.AssignedResources);

    /// <summary>
    /// Verifies whether the sets of assigned resources contain the same resources
    /// </summary>
    private static bool ObjectsAreEqual(IReadOnlyList<IOperatorAssignable> assignedResources1, IReadOnlyList<IOperatorAssignable> assignedResources2)
        => assignedResources1.All(r1 => assignedResources2.Any(r2 => r1.Id == r2.Id));

    #endregion

    #region Skill Test Tools

    /// <summary>
    /// Standard skill type creation context
    /// </summary>
    protected static SkillTypeCreationContext SkillTypeCreationContext
        => CreateSkillTypeCreationContext("Test Skill Type", TimeSpan.FromHours(42), new TestCapabilities(1));

    private static SkillTypeCreationContext CreateSkillTypeCreationContext(string name, TimeSpan duration, ICapabilities acquiredCapabilities)
        => new(name, acquiredCapabilities) { Duration = duration };

    /// <summary>
    /// Standard skill creation context referencing the standart skill type and standard operator
    /// </summary>
    protected static SkillCreationContext SkillCreationContext
        => CreateSkillCreationContext(Operator, SkillType, DateOnly.FromDateTime(DateTime.Today));

    /// <summary>
    /// Skill creation context for an expired skill referencing the standart skill type and standard operator
    /// </summary>
    protected static SkillCreationContext ExpiredSkillCreationContext
        => CreateSkillCreationContext(Operator, SkillType, DateOnly.MinValue);

    private static SkillCreationContext CreateSkillCreationContext(Operator @operator, SkillType type, DateOnly obtained)
        => new(@operator, type) { ObtainedOn = obtained };

    /// <summary>
    /// Standard skill type with id 1
    /// </summary>
    protected static SkillType SkillType => CreateSkillType(1, "Test Skill Type", TimeSpan.FromHours(42), new TestCapabilities(1));

    /// <summary>
    /// An updated skill type with id 1 and changed properties
    /// </summary>
    protected static SkillType UpdatedSkillType => CreateSkillType(1, "Updated Test Skill Type", TimeSpan.FromHours(1337), new TestCapabilities(2));

    /// <summary>
    /// An unknown skill type instance with id 0
    /// </summary>
    protected static SkillType UnknownSkillType => CreateSkillType(0, "", TimeSpan.Zero, NullCapabilities.Instance);

    private static SkillType CreateSkillType(int id, string name, TimeSpan duration, ICapabilities acquiredCapabilities)
        => new(name, acquiredCapabilities) { Id = id, Duration = duration };

    /// <summary>
    /// Standard skill with id 1 referencing the standard skill type and standard operator
    /// </summary>
    protected static Skill Skill => CreateSkill(1, SkillType, Operator, DateOnly.FromDateTime(DateTime.Today));

    /// <summary>
    /// Unknown skill with id 0 referencing the standard skill type and standard operator
    /// </summary>
    protected static Skill UnknownSkill => CreateSkill(0, SkillType, Operator, DateOnly.FromDateTime(DateTime.Today));

    private static Skill CreateSkill(long id, SkillType type, Operator @operator, DateOnly obtained)
        => new(type, @operator) { Id = id, ObtainedOn = obtained };

    /// <summary>
    /// Verifies whether the sets of assigned resources contain the same resources
    /// </summary>
    protected static bool ObjectsAreEqual(SkillType s1, SkillType s2)
        => s1.Id == s2.Id && s1.Name == s2.Name && s1.Duration == s2.Duration &&
        s1.AcquiredCapabilities.ProvidedBy(s2.AcquiredCapabilities) && s1.AcquiredCapabilities.Provides(s2.AcquiredCapabilities);

    /// <summary>
    /// Verifies whether the sets of assigned resources contain the same resources
    /// </summary>
    protected static bool ObjectsAreEqual(Skill s1, Skill s2)
        => s1.Id == s2.Id && s1.ObtainedOn == s2.ObtainedOn && ObjectsAreEqual(s1.Type, s2.Type) && ObjectsAreEqual(s1.Operator, s2.Operator);

    #endregion
}

/// <summary>
/// Capabilities to be used in the integration tests for the skill management
/// </summary>
public class TestCapabilities : CapabilitiesBase
{
    public TestCapabilities() { }

    public TestCapabilities(int number) => Number = number;

    public int Number { get; set; }

    protected override bool ProvidedBy(ICapabilities provided)
    {
        if (provided is not TestCapabilities providedTestCapabilities)
            return false;
        if (providedTestCapabilities.Number != Number)
            return false;
        return true;
    }
}
