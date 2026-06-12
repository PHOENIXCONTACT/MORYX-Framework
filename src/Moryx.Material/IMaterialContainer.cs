// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.States;

namespace Moryx.Material;

/// <summary>
/// Resource interface for material containers. A material container is the digital twin
/// of any physical container holding material in a cyber-physical system.
/// </summary>
/// <remarks>
/// Application engineers can extend this interface for domain-specific containers (e.g.,
/// order-linked, product-linked) without polluting the core abstraction.
/// </remarks>
public interface IMaterialContainer : IResource
{
    /// <summary>
    /// Optional scannable identity of the physical container (e.g., barcode, QR code).
    /// May be null for "virtual" containers in <see cref="RequestedState"/>.
    /// </summary>
    IIdentity? Identity { get; set; }

    /// <summary>
    /// Reference to the container's content. Subclasses may enrich this with stronger
    /// typing (e.g., linking to a <c>ProductType</c>).
    /// </summary>
    string? Material { get; set; }

    /// <summary>
    /// Current filling level / amount of material held by this container.
    /// </summary>
    decimal Quantity { get; set; }

    /// <summary>
    /// Optional unit of <see cref="Quantity"/> (e.g. "kg", "pcs"). Free-form to keep
    /// the core lightweight; application engineers can constrain via custom hooks.
    /// </summary>
    string? Unit { get; set; }

    /// <summary>
    /// Current lifecycle state of the container.
    /// </summary>
    MaterialContainerStateBase State { get; }

    /// <summary>
    /// Raised when <see cref="Material"/> changes.
    /// </summary>
    event EventHandler<MaterialChangedEventArgs>? MaterialChanged;

    /// <summary>
    /// Raised when <see cref="Quantity"/> changes.
    /// </summary>
    event EventHandler<FillingLevelChangedEventArgs>? FillingLevelChanged;

    /// <summary>
    /// Raised when <see cref="State"/> changes.
    /// </summary>
    event EventHandler<StateChangedEventArgs>? StateChanged;
}