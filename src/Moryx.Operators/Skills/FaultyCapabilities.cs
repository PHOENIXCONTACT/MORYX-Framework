// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.Operators.Localizations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Operators.Skills;

/// <summary>
/// Represents a capability that could not be reconstructed. 
/// This can happen whenever a capability is serialized and unserialized again, 
/// e.g. when a type was renamed without adjusting the already existing database 
/// entries.
/// </summary>
public class FaultyCapabilities : CapabilitiesBase
{
    /// <summary>
    /// Name of the original capability type, if known.
    /// </summary>
    [Display(Name = nameof(Strings.ORIGINAL_TYPE), ResourceType = typeof(Strings))]
    [ReadOnly(true)]
    public string? OriginalType { get; set; }

    /// <summary>
    /// The exception, if any, that occurred while trying to construct the original capability.
    /// </summary>
    [Display(Name = nameof(Strings.EXCEPTION), ResourceType = typeof(Strings))]
    [ReadOnly(true)]
    public string? Exception { get; set; }

    protected override bool ProvidedBy(ICapabilities provided) => false;
}

