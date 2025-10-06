// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Localizations;
using Moryx.Serialization;
using Moryx.Tools;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Moryx.Operators.Skills;

/// <summary>
/// The simplest possible resource to be used for operator assignements implementing the <see cref="IOperatorAssignable"/> interface.
/// </summary>
[Display(Name = nameof(Strings.ATTENDABLE_RESOURCE_TITLE), Description = nameof(Strings.ATTENDABLE_RESOURCE_DESCRIPTION), ResourceType = typeof(Strings))]
public class AttendableResource : Resource, IOperatorAssignable
{
    private static readonly IReadOnlyCollection<Type> _capabilityTypes = ReflectionTool.GetPublicClasses<ICapabilities>()
            .Where(t => !t.IsAbstract && !t.IsGenericType).ToImmutableArray();

    private string configuredRequiredSkill;

    public AttendableResource()
    {
        ResetRequiredSkills();
    }

    [MemberNotNull([nameof(configuredRequiredSkill), nameof(RequiredSkills)])]
    private void ResetRequiredSkills()
    {
        configuredRequiredSkill = nameof(NullCapabilities);
        RequiredSkills = NullCapabilities.Instance;
    }

    /// <summary>
    /// Entry serialized property to configure the required skill for this resource
    /// </summary>
    [Display(Name = nameof(Strings.SKILLNAME), Description = nameof(Strings.SKILLNAME_DESCRIPTION), ResourceType = typeof(Strings))]
    [EntrySerialize, DataMember, PossibleTypes(typeof(ICapabilities))]
    public string ConfiguredRequiredSkill
    {
        get => configuredRequiredSkill;
        set
        {
            configuredRequiredSkill = value;
            var type = _capabilityTypes.SingleOrDefault(t => t.Name == configuredRequiredSkill);
            if (type is null)
            {
                ResetRequiredSkills();
                return;
            }
            var instance = CreateCapabilitiesInstance(type);
            if (instance is null)
            {
                ResetRequiredSkills();
                return;
            }

            RequiredSkills = (ICapabilities)instance;
        }
    }

    private static object? CreateCapabilitiesInstance(Type type)
    {
        if (type == typeof(NullCapabilities))
        {
            return NullCapabilities.Instance;
        }
        return Activator.CreateInstance(type);
    }

    /// <inheritdoc/>
    public ICapabilities RequiredSkills { get; private set; } = NullCapabilities.Instance;

    /// <inheritdoc/>
    public void AttendanceChanged(IReadOnlyList<AttendanceChangedArgs> attandances) { }
}

