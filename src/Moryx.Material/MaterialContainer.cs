// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.States;

namespace Moryx.Material;

/// <summary>
/// Default base class for <see cref="IMaterialContainer"/> implementations.
/// </summary>
/// <remarks>
/// State changes are mediated through <see cref="TransitionTo(MaterialContainerStateBase)"/>;
/// this is internal-friendly so the management module can drive transitions while the
/// resource still owns the state. Application engineers may extend this class to add
/// custom properties and behavior.
/// </remarks>
[DataContract]
public abstract class MaterialContainer : Resource, IMaterialContainer
{
    private MaterialContainerStateBase _state = new RequestedState();
    private string? _material;
    private decimal _quantity;

    /// <inheritdoc />
    [DataMember]
    public IIdentity? Identity { get; set; }

    /// <inheritdoc />
    [DataMember]
    public string? Material
    {
        get => _material;
        set
        {
            if (string.Equals(_material, value, StringComparison.Ordinal))
                return;

            var oldMaterial = _material;
            _material = value;
            MaterialChanged?.Invoke(this, new MaterialChangedEventArgs(this, oldMaterial, value));
            RaiseResourceChanged();
        }
    }

    /// <inheritdoc />
    [DataMember]
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value)
                return;

            var oldQuantity = _quantity;
            _quantity = value;
            FillingLevelChanged?.Invoke(this, new FillingLevelChangedEventArgs(this, oldQuantity, value));
            RaiseResourceChanged();
        }
    }

    /// <inheritdoc />
    [DataMember]
    public string? Unit { get; set; }

    /// <inheritdoc />
    [DataMember]
    public MaterialContainerStateBase State => _state;

    /// <inheritdoc />
    public event EventHandler<MaterialChangedEventArgs>? MaterialChanged;

    /// <inheritdoc />
    public event EventHandler<FillingLevelChangedEventArgs>? FillingLevelChanged;

    /// <inheritdoc />
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Transitions the container to a new state and raises <see cref="StateChanged"/>.
    /// Intended to be called by the management module's state handler or by subclasses.
    /// </summary>
    protected internal virtual void TransitionTo(MaterialContainerStateBase newState)
    {
        if (newState == null)
            throw new ArgumentNullException(nameof(newState));

        var oldState = _state;
        if (ReferenceEquals(oldState, newState))
            return;

        newState.EnteredAt = DateTime.UtcNow;
        _state = newState;
        StateChanged?.Invoke(this, new StateChangedEventArgs(this, oldState, newState));
        RaiseResourceChanged();
    }
}