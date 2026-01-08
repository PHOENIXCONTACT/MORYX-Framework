// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents a skill.
/// </summary>
/// <remarks>
/// Creates a new skill
/// </remarks>
/// <param name="type">The type of skill an operator obtained</param>
/// <param name="operator">The operator that obtained the skill</param>
public class Skill(SkillType type, Operator @operator) : IPersistentObject
{
    /// <summary>
    /// Gets or sets the ID of the skill.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the date when the skill was obtained.
    /// </summary>
    public DateOnly ObtainedOn { get; set; }

    /// <summary>
    /// Gets or sets the type of the skill.
    /// </summary>
    public SkillType Type { get; set; } = type;

    /// <summary>
    /// Gets or sets the operator of the skill.
    /// </summary>
    public Operator Operator { get; set; } = @operator;

    /// <summary>
    /// Indicates whether this skill is expired
    /// </summary>
    public bool IsExpired => ObtainedOn.AddDays(Type.Duration.Days) < DateOnly.FromDateTime(DateTime.Today);
}
