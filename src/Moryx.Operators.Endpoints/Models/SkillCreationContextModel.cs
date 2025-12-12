// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Operators.Endpoints.Models;

/// <summary>
/// Represents the context for creating a skill.
/// </summary>
public class SkillCreationContextModel
{
    /// <summary>
    /// Gets or sets the date when the skill was obtained.
    /// </summary>
    public DateOnly ObtainedOn { get; set; }

    /// <summary>
    /// Gets or sets the ide of the skill type of the skill to be created.
    /// </summary>
    public long TypeId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the operator of the skill to be created.
    /// </summary>
    public string? OperatorIdentifier { get; set; }
}
