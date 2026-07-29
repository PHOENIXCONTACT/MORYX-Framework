// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Linking;
using Moryx.Material.States;
using Moryx.Serialization;
using ValidationContext = Moryx.Material.Linking.ValidationContext;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Default base class for <see cref="IOrderLinkedMaterialContainer"/> implementations.
/// </summary>
[DataContract]
public abstract class OrderLinkedMaterialContainer : MaterialContainer, IOrderLinkedMaterialContainer
{
    #region IOrderLinkedMaterialContainer
    private OrderReference? _linkedOrder;
    /// <inheritdoc />
    [DataMember]
    public OrderReference? LinkedOrder
    {
        get => _linkedOrder;
        set
        {
            var previous = _linkedOrder;
            _linkedOrder = value;
            if (!previous.ValueEquals(value))
            {
                RaiseResourceChanged();
            }
        }
    }

    /// <inheritdoc />
    public virtual async Task RequestOrderLinkAsync(string orderNumber, string? operationNumber = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(orderNumber))
        {
            throw new ArgumentException("Order number is required.", nameof(orderNumber));
        }

        var request = new OrderLinkingRequest(orderNumber, operationNumber, _linkedOrder);
        await ExecuteLinkingProtocolAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task RequestOrderUnlinkAsync(CancellationToken cancellationToken = default)
    {
        if (_linkedOrder == null)
        {
            return;
        }

        var request = new OrderLinkingRequest(_linkedOrder);
        await ExecuteLinkingProtocolAsync(request, cancellationToken);
    }

    /// <summary>
    /// Drives the two-phase linking protocol against any registered listeners
    /// (typically the <c>LinkingHookManager</c>).
    /// </summary>
    protected virtual async Task ExecuteLinkingProtocolAsync(OrderLinkingRequest request, CancellationToken cancellationToken)
    {
        var listeners = OrderLinkRequested;
        if (listeners is null)
        {
            // No integration is currently active.
            // We "fail" gracefully in this case and apply the reference directly without validation. While this
            // could circumvent validation hooks in certain cases, it prevents process interruptions and even allows
            // for using order-linked material containers without the sophisticated integration infrastructure.
            ApplyLink(request, new ValidationContext(), createReferenceLocally: true);
            return;
        }

        ValidationContext? finalContext = null;
        OrderReference? finalReference = null;

        Task ResponseHandler(LinkingResponse response)
        {
            finalContext = response.Context;
            finalReference = response.Reference as OrderReference;
            return Task.CompletedTask;
        }

        var args = new OrderLinkRequestEventArgs(request, ResponseHandler);
        listeners.Invoke(this, args);

        // Await the response if it was scheduled asynchronously.
        if (args.ResponseCallback != null && finalContext == null)
        {
            await Task.Yield();
        }

        // No validation context delivered OR context indicates an error: treat as failure.
        if (finalContext == null || finalContext.HasErrors)
        {

            await HandleValidationErrors(finalContext);
            return;
        }

        // ToDo: Add processing of validation limitations

        ApplyLink(request, finalContext, createReferenceLocally: false, providedReference: finalReference);
    }

    /// <summary>
    /// Applies the linking decision to this container and raises <see cref="OrderLinkApplied"/>.
    /// </summary>
    protected void ApplyLink(OrderLinkingRequest request, ValidationContext context, bool createReferenceLocally, OrderReference? providedReference = null)
    {
        OrderReference? applied = null;

        if (request.IsUnlink)
        {
            applied = _linkedOrder = null;
        }
        else if (providedReference is not null)
        {
            applied = _linkedOrder = providedReference;
        }
        else if (createReferenceLocally && !string.IsNullOrEmpty(request.OrderNumber))
        {
            applied = _linkedOrder = new OrderReference(request.OrderNumber, request.OperationNumber);
        }
        else
        {
            throw new InvalidOperationException($"Cannot apply {nameof(OrderReference)} " +
                $"{(providedReference is null ? "" : $"{providedReference.OrderNumber}-{providedReference.OperationNumber}")} " +
                $"for {nameof(OrderLinkingRequest)} {request.OrderNumber}-{request.OperationNumber}");
        }

        RaiseResourceChanged();

        OrderLinkApplied?.Invoke(this, new OrderLinkAppliedEventArgs(request, context, applied));
    }

    protected virtual Task HandleValidationErrors(ValidationContext? context) => Task.CompletedTask;

    /// <inheritdoc />
    public event EventHandler<OrderLinkRequestEventArgs>? OrderLinkRequested;

    /// <inheritdoc />
    public event EventHandler<OrderLinkAppliedEventArgs>? OrderLinkApplied;
    #endregion

    /// <summary>
    /// Resource constructor method bringing a <see cref="MaterialContainer"/> from the <see cref="StateClassification.Uninitialized"/>
    /// state into the <see cref="StateClassification.Available"/> state using provided information and requesting to linking it to
    /// an order right away.
    /// </summary>
    /// <param name="identityType">Optional identity instance used to represent the container identity.</param>
    /// <param name="identity">Optional identifier value assigned to <paramref name="identityType"/>.</param>
    /// <param name="material">Optional material reference contained in the container.</param>
    /// <param name="quantity">Initial quantity contained in the container.</param>
    /// <param name="unit">Optional unit of <paramref name="quantity"/>.</param>
    [ResourceConstructor]
    [Display(Name = "Material Registration", Description = "Create a material container that is linked to an order")]
    public virtual Task With(
        [Display(Name = "Order Number", Description = "Order number this container is linked to"), PossibleOrderNumbers] string orderNumber,
        [Display(Name = "Operation Number", Description = "Operation number this container is linked to"), PossibleOperationNumbers] string? operationNumber = null,
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
        return RequestOrderLinkAsync(orderNumber, operationNumber);
    }
}
