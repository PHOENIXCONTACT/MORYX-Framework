// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents the changes in a skill type.
/// </summary>
/// <remarks>
/// Create a new instance of skill type changed arguments
/// </remarks>
/// <param name="change">The kind of change that happend</param>
/// <param name="skillType">The skill type that changed</param>
public class SkillTypeChangedEventArgs(SkillTypeChange change, SkillType skillType) : EventArgs
{
    /// <summary>
    /// Gets or sets the change in the skill type.
    /// </summary>
    public SkillTypeChange Change { get; set; } = change;

    /// <summary>
    /// Gets or sets the skill type.
    /// </summary>
    public SkillType SkillType { get; set; } = skillType;
}
