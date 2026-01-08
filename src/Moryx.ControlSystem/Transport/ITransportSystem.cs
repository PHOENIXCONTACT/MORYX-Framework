// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Processes;

namespace Moryx.ControlSystem.Transport;

/// <summary>
/// Resource that can route products or workpiece groups within the machine
/// </summary>
public interface ITransportSystem : IResource
{
    /// <summary>
    /// Modes supported by this routing resource
    /// </summary>
    TransportMode SupportedModes { get; }

    /// <summary>
    /// Get all process groups managed by this resource
    /// </summary>
    IEnumerable<IProcessHolderGroup> ManagedGroups { get; }

    /// <summary>
    /// Update the targets of a group
    /// </summary>
    void UpdateTargets(IProcessHolderPosition holderPosition, IEnumerable<IResource> targets);

    /// <summary>
    /// Update the targets of a group
    /// </summary>
    void UpdateTargets(IProcessHolderGroup holderGroup, IEnumerable<IResource> targets);

    /// <summary>
    /// Event raised when a group was added
    /// </summary>
    event EventHandler<IProcessHolderGroup> GroupAdded;

    /// <summary>
    /// Event raised, when a group was updated
    /// </summary>
    event EventHandler<IProcessHolderGroup> GroupUpdated;

    /// <summary>
    /// Event raised, when a group was removed
    /// </summary>
    event EventHandler<IProcessHolderGroup> GroupRemoved;
}