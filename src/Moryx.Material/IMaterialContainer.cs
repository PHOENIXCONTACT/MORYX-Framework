// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Events;
using Moryx.Material.States;

namespace Moryx.Material;

// ToDo: Should IIdentifiableObject be part of the interface or only the base class?
/// <summary>
/// Resource interface for material containers. A material container is the digital twin
/// of any physical container holding material in a cyber-physical system.
/// </summary>
/// <remarks>
/// Application engineers can extend this interface for domain-specific containers (e.g.,
/// order-linked, product-linked) without polluting the core abstraction.
/// </remarks>
public interface IMaterialContainer : IResource, IIdentifiableObject
{
    /// <summary>
    /// The hosting resource that currently holds or displays this container (e.g. a carrier, machine, shelf slot).
    /// Implementations should reference a resource that represents the physical or logical host.
    /// </summary>
    IResource? ContainerHost { get; set; }

    /// <summary>
    /// Denotes the container's content, subclasses are intended to enrich this with stronger
    /// typing (e.g., linking to a <c>ProductType</c>).
    /// </summary>
    string? Material { get; set; }

    /// <summary>
    /// Current filling level / amount of material held by this container.
    /// </summary>
    decimal Quantity { get; set; }

    /// <summary>
    /// Optional unit of <see cref="Quantity"/> (e.g. "kg", "pcs").
    /// </summary>
    string? Unit { get; set; }

    /// <summary>
    /// Current lifecycle state classification of the container.
    /// </summary>
    StateClassification State { get; }

    /// <summary>
    /// Raised when <see cref="Material"/> changes.
    /// </summary>
    event EventHandler<MaterialChangedEventArgs>? MaterialChanged;

    /// <summary>
    /// Raised when <see cref="Quantity"/> changes.
    /// </summary>
    event EventHandler<FillingLevelChangedEventArgs>? FillingLevelChanged;

    /// <summary>
    /// Raised when <see cref="StateClassification"/> changes.
    /// </summary>
    event EventHandler<StateChangedEventArgs>? StateChanged;
}
