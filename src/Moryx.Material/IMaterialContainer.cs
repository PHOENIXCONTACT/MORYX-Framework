// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.States;

namespace Moryx.Material;

// ToDo: Should IIdentifiableObject be part of the interface or only the base class?
// ToDo: Should we split the interface into IMaterialContainer and IStatefulMaterialContainer, putting state information and transition method into the latter?
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
    string? Material { get; }

    /// <summary>
    /// Current filling level / amount of material held by this container.
    /// </summary>
    double Quantity { get; }

    /// <summary>
    /// Optional unit of <see cref="Quantity"/> (e.g. "kg", "pcs").
    /// </summary>
    string? Unit { get; }

    /// <summary>
    /// Current lifecycle state classification of the container.
    /// </summary>
    StateClassification State { get; }

    /// <summary>
    /// Information object for the current lifecycle state of the container.
    /// </summary>
    StateInformation? StateInformation { get; }

    /// <summary>
    /// Applies a material update to this container and raises <see cref="MaterialUpdated"/> when a value changes.
    /// </summary>
    /// <param name="update">Update describing the material properties to change.</param>
    void UpdateMaterial(MaterialUpdate update);

    /// <summary>
    /// Applies a lifecycle state transition to this container.
    /// </summary>
    /// <param name="stateInformation">State information describing the target lifecycle state.</param>
    /// <exception cref="InvalidOperationException">Throws if the intended target state cannot be reached from the current state</exception>
    void TransitionTo(StateInformation stateInformation);

    /// <summary>
    /// Raised when <see cref="Material"/> and/or <see cref="Quantity"/> changes.
    /// </summary>
    event EventHandler<MaterialUpdatedEventArgs>? MaterialUpdated;

    /// <summary>
    /// Raised when <see cref="StateClassification"/> changes.
    /// </summary>
    event EventHandler<StateChangedEventArgs>? StateChanged;
}
