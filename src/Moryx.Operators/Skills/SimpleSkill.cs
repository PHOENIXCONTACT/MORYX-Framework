// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.Operators.Localizations;
using System.ComponentModel.DataAnnotations;
using Moryx.Operators.Localizations;

namespace Moryx.Operators.Skills;

/// <summary>
/// A <see cref="SimpleSkill"/> representing cababilities of a user which are only
/// described by the name of the skill. Use this skill in any situations where you don't
/// want or need to enrich the information of what an <see cref="Operator"/> is capable of
/// beyond the name of the skill they have acquired.
/// </summary>
[Display(Name = "Named Skill", Description = "A simple skill providing only a name and no further specification of the capabilities required or provided")]
public class SimpleSkill : CapabilitiesBase
{
    /// <summary>
    /// The name of the skill aquired when holding these capabilities
    /// </summary>
    [Display(Name = nameof(Strings.SimpleSkill_Name), ResourceType = typeof(Strings))]
    public string? Name { get; set; }

    /// <inheritdoc/>
    protected override bool ProvidedBy(ICapabilities provided)
    {
        var simpleSkill = provided as SimpleSkill;
        if (simpleSkill == null)
            return false;

        if (string.IsNullOrEmpty(Name) || !Equals(Name, simpleSkill.Name))
            return false;

        return true;
    }
}
