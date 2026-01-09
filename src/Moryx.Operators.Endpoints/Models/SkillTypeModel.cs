// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Operators.Endpoints.Models;

/// <summary>
/// Represents a skill type.
/// </summary>
public class SkillTypeModel
{
    /// <summary>
    /// Gets or sets the ID of the skill type.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the skill type.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the duration of the skill type.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Capabilities associated with this type
    /// </summary>
    public Entry? Capabilities { get; set; }
}
