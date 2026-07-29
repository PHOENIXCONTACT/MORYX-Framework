// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Factory;
using Moryx.Material.States;
using Moryx.Serialization;
using Moryx.StateMachines;

namespace Moryx.Material;

/// <summary>
/// Default base class for <see cref="IMaterialContainer"/> implementations.
/// </summary>
/// <remarks>
/// Application engineers may extend this class to add custom properties and behavior.
/// </remarks>
[DataContract]
[EntryVisualization("", "inbox")]
public abstract class MaterialContainer : Resource, IMaterialContainer, IStateContext
{
    #region IMaterialContainer

    // ToDo: If ResourceRelationType.Aggregation should not be used alone, a new type must be introduced
    /// <inheritdoc />
    [ResourceReference(ResourceRelationType.Aggregation, ResourceReferenceRole.Source, "Container Host", IsRequired = false)]
    [Display(Name = "Host", Description = "Resource that hosts or holds this container (e.g., carrier, machine, shelf slot).")]
    public IResource? ContainerHost { get; set; }

    /// <summary>
    /// Optional scannable identity of the physical container (e.g., barcode or QR code).
    /// May be <c>null</c> for virtual containers in <see cref="RequestedStateInformation"/> or unlabelled container types.
    /// </summary>
    [DataMember]
    [EntrySerialize]
    [Display(Name = "Container Identity", Description = "Optional scannable identity of the physical container (e.g., barcode, QR code). May be null for virtual or unlabelled containers."), PossibleTypes(typeof(IIdentity))]
    public virtual IIdentity? Identity { get; set; }

    // ToDo: Should we lock modifications in pre-advises and deregistered state?
    /// <inheritdoc />
    [DataMember]
    [EntrySerialize, ReadOnly(true)]
    [Display(Name = "Material", Description = "Material reference contained in this container (e.g., product number).")]
    public virtual string? Material {  get; protected set; }

    /// <inheritdoc />
    [DataMember]
    [EntrySerialize, ReadOnly(true)]
    [Display(Name = "Quantity", Description = "Current amount of material held by this container.")]
    public virtual double Quantity { get; protected set; }

    /// <inheritdoc />
    [DataMember]
    [EntrySerialize]
    [Display(Name = "Unit", Description = "Unit of measure for the quantity (e.g., kg, pcs).")]
    public virtual string? Unit { get; set; }

    /// <inheritdoc />
    [EntrySerialize, ReadOnly(true)]
    [Display(Name = "State", Description = "Current lifecycle state classification of the container.")]
    public StateClassification State => _state?.Classification ?? StateClassification.Uninitialized;

    // ToDo: Can this be private set even if written from db?
    /// <summary>
    /// Detailed lifecycle state data for the current <see cref="State"/>.
    /// </summary>
    [DataMember]
    [EntrySerialize, ReadOnly(true)]
    [Display(Name = "State Information", Description = "Information about the current state of the container.")]
    public StateInformation? StateInformation { get; set; }

    /// <inheritdoc />
    public void UpdateMaterial(MaterialUpdate update)
    {
        if (IsNoOp(update))
        {
            return;
        }

        var eventArgs = new MaterialUpdatedEventArgs() { Kind = update.Kind };
        // ToDo: Do we need an extra flag for unit? Is empty unit allowed?
        if (update.Kind.HasFlag(UpdateKind.MaterialType))
        {
            eventArgs.OldMaterial = Material;
            eventArgs.NewMaterial = Material = update.Material;
            Unit = update.Unit;
        }

        if (update.Kind.HasFlag(UpdateKind.FillingLevel)) {
            if (update.Kind.HasFlag(UpdateKind.Relative))
            {
                eventArgs.OldQuantity = Quantity;
                eventArgs.NewQuantity = Quantity -= update.Quantity;
            }
            else
            {
                eventArgs.OldQuantity = Quantity;
                eventArgs.NewQuantity = Quantity = update.Quantity;
            }
        }

        RaiseResourceChanged();
        MaterialUpdated?.Invoke(this, eventArgs);
    }

    private bool IsNoOp(MaterialUpdate update)
    {
        return update.Kind == UpdateKind.NoOperation ||
            update.Kind == UpdateKind.MaterialType && string.Equals(update.Material, Material, StringComparison.Ordinal) ||
            update.Kind == UpdateKind.FillingLevel && update.Quantity == Quantity;
    }

    /// <inheritdoc/>
    public void TransitionTo(StateInformation stateInformation)
    {
        ArgumentNullException.ThrowIfNull(stateInformation);
        if (_state == null)
        {
            throw new InvalidOperationException("The material container state machine is not initialized.");
        }

        var oldStateInformation = StateInformation;
        _state.Advance(stateInformation);
        StateChanged?.Invoke(this, new StateChangedEventArgs(oldStateInformation, stateInformation));
        OnStateChanged();
    }

    /// <inheritdoc />
    public event EventHandler<MaterialUpdatedEventArgs>? MaterialUpdated;

    /// <inheritdoc />
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    #endregion

    #region IStateContext

    private MaterialContainerState? _state;
    private readonly Lock _stateLock = new();

    /// <inheritdoc/>
    void IStateContext.SetState(StateBase state)
    {
        _state = (MaterialContainerState)state;

        if (state is UninitializedState)
        {
            return;
        }

        OnStateChanged();
    }

    /// <summary>
    /// Called after the <see cref="MaterialContainer"/> changed its lifecycle state.
    /// </summary>
    protected virtual void OnStateChanged()
    {
    }

    #endregion

    #region Resource
    /// <inheritdoc/>
    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        await base.OnInitializeAsync(cancellationToken);
        StateMachine.ForContext(this).With<MaterialContainerState>();
    }
    #endregion

    /// <summary>
    /// Resource constructor method bringing a <see cref="MaterialContainer"/> from the <see cref="StateClassification.Uninitialized"/>
    /// state into the <see cref="StateClassification.Requested"/> state using information provided by the <paramref name="request"/>.
    /// </summary>
    /// <param name="request">Information specifying the material request.</param>
    [ResourceConstructor]
    [Display(Name = "Material Request", Description = "Create a virtual container to request material")]
    public virtual void With([Display(Name = "Request Information", Description = "Specifications for the material request")] MaterialRequest request)
    {
        StateInformation = new RequestedStateInformation()
        {
            RequestId = request.Id,
            ExpectedArrival = request.ExpectedArrival,
        };
        Identity = request.ContainerIdentity;
        Material = request.Material;
        Quantity = request.RequestedQuantity;
        Unit = request.Unit;
    }

    /// <summary>
    /// Resource constructor method bringing a <see cref="MaterialContainer"/> from the <see cref="StateClassification.Uninitialized"/>
    /// state into the <see cref="StateClassification.Available"/> state using provided information.
    /// </summary>
    /// <param name="identityType">Optional identity instance used to represent the container identity.</param>
    /// <param name="identity">Optional identifier value assigned to <paramref name="identityType"/>.</param>
    /// <param name="material">Optional material reference contained in the container.</param>
    /// <param name="quantity">Initial quantity contained in the container.</param>
    /// <param name="unit">Optional unit of <paramref name="quantity"/>.</param>
    [ResourceConstructor]
    [Display(Name = "Material Registration", Description = "Create a material container in the system")]
    public virtual void With(
        [Display(Name = "Identity Kind", Description = "Type of identity for the Container (e.g. Serialnumber)"), PossibleTypes(typeof(IIdentity))] IIdentity? identityType = null,
        [Display(Name = "Identity", Description = "Identity unique to the Container (e.g. 123-456-789)")] string? identity = null,
        [Display(Name = "Material", Description = "The material in the container")] string? material = null,
        [Display(Name = "Quantity", Description = "Amount of material in the container")] double quantity = 0,
        [Display(Name = "Unit", Description = "Unit the quantity is given in")] string? unit = null)
    {
        StateInformation = new AvailableStateInformation();
        Identity = identityType;
        Identity?.SetIdentifier(identity);
        Material = material;
        Quantity = quantity;
        Unit = unit;
    }
}
