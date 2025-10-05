// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents the context for creating a skill.
/// </summary>
/// <remarks>
/// Creates a new creation context for a skill
/// </remarks>
/// <param name="operator">The operator that obtains the skill.</param>
/// <param name="type">The type of the skill the operator obtains.</param>
public class SkillCreationContext(Operator @operator, SkillType type)
{
    /// <summary>
    /// Gets or sets the date when the skill was obtained.
    /// </summary>
    public DateOnly ObtainedOn { get; set; }

    /// <summary>
    /// Gets or sets the skill type of the skill to be created.
    /// </summary>
    public SkillType Type { get; set; } = type;

    /// <summary>
    /// Gets or sets the operator of the skill to be created.
    /// </summary>
    public Operator Operator { get; set; } = @operator;
}
