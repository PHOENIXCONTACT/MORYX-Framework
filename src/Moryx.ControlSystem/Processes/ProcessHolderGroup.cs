// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Properties;

namespace Moryx.ControlSystem.Processes;

/// <summary>
/// Default implementation for <see cref="IProcessHolderGroup{IProcessHolderPosition}"/>
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.ProcessHolderGroup_DisplayName), Description = nameof(Strings.ProcessHolderGroup_Description), ResourceType = typeof(Strings))]
public class ProcessHolderGroup : Resource, IProcessHolderGroup
{
    /// <summary>
    /// All positions of this carrier
    /// </summary>
    [ReferenceOverride(nameof(Children))]
    public IReferences<IProcessHolderPosition> Positions { get; set; }

    /// <inheritdoc />
    IEnumerable<IProcessHolderPosition> IProcessHolderGroup.Positions => Positions;

    /// <inheritdoc />
    public void Reset()
    {
        foreach (var position in Positions)
        {
            position.Reset();
        }
    }
}