// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// Requirement raised by a hook during linking validation that must be fulfilled
/// before the linking operation can be applied.
/// </summary>
public interface ILinkingRequirement
{
    /// <summary>
    /// Whether this requirement is automatically applicable by the system or requires manual operator input.
    /// </summary>
    RequirementMode Mode { get; }

    /// <summary>
    /// Whether this requirement was fulfilled.
    /// </summary>
    bool IsFulfilled { get; set; }
}

/// <summary>
/// Mode of a <see cref="ILinkingRequirement"/>.
/// </summary>
public enum RequirementMode
{
    /// <summary>System can apply default fulfillment.</summary>
    Automatic = 0,

    /// <summary>Operator must explicitly fulfill.</summary>
    Manual = 1
}