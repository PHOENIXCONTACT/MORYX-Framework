// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Operators.Endpoints.Models;

/// <summary>
/// Represents the context for creating a skill type.
/// </summary>
public class SkillTypeCreationContextModel
{
    /// <summary>
    /// Gets or sets the name of the skill type.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the duration of the skill type.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Capabilities of this skill type
    /// </summary>
    public Entry? Capabilities { get; set; }
}
