// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents the changes in a skill.
/// </summary>
/// <remarks>
/// Creates a new instance of skill changed event args
/// </remarks>
/// <param name="skill">The skill that changed</param>
public class SkillChangedEventArgs(SkillChange change, Skill skill) : EventArgs
{
    /// <summary>
    /// Gets or sets the change in the skill.
    /// </summary>
    public SkillChange Change { get; set; } = change;

    /// <summary>
    /// Gets or sets the skill.
    /// </summary>
    public Skill Skill { get; set; } = skill;
}
