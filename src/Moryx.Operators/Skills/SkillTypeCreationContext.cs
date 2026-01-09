// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents the context for creating a skill type.
/// </summary>
/// <remarks>
/// Creates a new creation context for a skill type
/// </remarks>
/// <param name="name">The name of the <see cref="SkillType"/> to be created.</param>
/// <param name="acquiredCapabilities">The <see cref="ICapabilities"/> that are acquired through the <see cref="SkillType"/> to be created.</param>
public class SkillTypeCreationContext(string name, ICapabilities acquiredCapabilities)
{
    /// <summary>
    /// Gets or sets the name of the skill type.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the duration of the skill type.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Capabilities acquired with this skill
    /// </summary>
    public ICapabilities AcquiredCapabilities { get; set; } = acquiredCapabilities;
}
