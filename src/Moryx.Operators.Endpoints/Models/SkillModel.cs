// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Endpoints.Models;

/// <summary>
/// Represents a skill.
/// </summary>
public class SkillModel
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
    /// Gets or sets the date when the skill will expire.
    /// </summary>
    public DateOnly ExpiresOn { get; set; }

    /// <summary>
    /// Gets or sets the id of the type of the skill.
    /// </summary>
    public long TypeId { get; set; }

    /// <summary>
    /// Gets or sets the identifeir of the operator of the skill.
    /// </summary>
    public string? OperatorIdentifier { get; set; }

    /// <summary>
    /// Indicates whether this skill is expired
    /// </summary>
    public bool IsExpired { get; set; }
}
