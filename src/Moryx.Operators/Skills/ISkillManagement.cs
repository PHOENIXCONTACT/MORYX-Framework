// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents the management of skills.
/// </summary>
public interface ISkillManagement
{
    /// <summary>
    /// Gets the list of skills.
    /// </summary>
    IReadOnlyList<Skill> Skills { get; }

    /// <summary>
    /// Creates a skill.
    /// </summary>
    /// <param name="skill">The context for creating the skill.</param>
    /// <returns>The created skill.</returns>
    Skill CreateSkill(SkillCreationContext skill);

    /// <summary>
    /// Deletes a skill.
    /// </summary>
    /// <param name="id">The ID of the skill to delete.</param>
    void DeleteSkill(long id);

    /// <summary>
    /// Gets the list of skill types.
    /// </summary>
    IReadOnlyList<SkillType> SkillTypes { get; }

    /// <summary>
    /// Creates a skill type.
    /// </summary>
    /// <param name="skillType">The context for creating the skill type.</param>
    /// <returns>The created skill type.</returns>
    SkillType CreateSkillType(SkillTypeCreationContext skillType);

    /// <summary>
    /// Updates a skill type.
    /// </summary>
    /// <param name="skillType">The skill type to update.</param>
    void UpdateSkillType(SkillType skillType);

    /// <summary>
    /// Deletes a skill type.
    /// </summary>
    /// <param name="id">The ID of the skill type to delete.</param>
    void DeleteSkillType(long id);

    /// <summary>
    /// Occurs when a skill has changed.
    /// </summary>
    event EventHandler<SkillChangedEventArgs> SkillChanged;

    /// <summary>
    /// Occurs when a skill type has changed.
    /// </summary>
    event EventHandler<SkillTypeChangedEventArgs> SkillTypeChanged;
}
