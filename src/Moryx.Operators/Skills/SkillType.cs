// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.Operators.Localizations;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents a skill type.
/// </summary>
/// <remarks>
/// Creates a new skill type.
/// </remarks>
/// <param name="name">The name of this skill type</param>
/// <param name="name">The capabilities acquired with this skill type</param>
public class SkillType(string name, ICapabilities acquiredCapabilities) : IPersistentObject
{
    /// <summary>
    /// Gets or sets the ID of the skill type.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the skill type.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the duration of the skill type.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Capabilities acquired with this skill type.
    /// </summary>
    [Display(Name = nameof(Strings.ACQUIRES_CAPABILITIES), ResourceType = typeof(Strings))]
    public ICapabilities AcquiredCapabilities { get; set; } = acquiredCapabilities;
}

