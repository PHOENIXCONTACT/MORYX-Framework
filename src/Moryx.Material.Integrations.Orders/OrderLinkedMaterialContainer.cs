// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// Default base class for <see cref="IOrderLinkedMaterialContainer"/> implementations.
/// </summary>
[DataContract]
public abstract class OrderLinkedMaterialContainer : MaterialContainer, IOrderLinkedMaterialContainer
{
    private OrderReference? _linkedOrder;

    /// <inheritdoc />
    [DataMember]
    public OrderReference? LinkedOrder
    {
        get => _linkedOrder;
        protected set
        {
            _linkedOrder = value;
            RaiseResourceChanged();
        }
    }

    /// <inheritdoc />
    public virtual async Task RequestOrderLinkAsync(string orderNumber, string? operationNumber = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(orderNumber))
            throw new ArgumentException("Order number is required.", nameof(orderNumber));

        var request = new OrderLinkingRequest(orderNumber, operationNumber, _linkedOrder);
        await ExecuteLinkingProtocolAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task RequestOrderUnlinkAsync(CancellationToken cancellationToken = default)
    {
        if (_linkedOrder == null)
            return;

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
        if (listeners == null)
        {
            // No integration is currently active. Apply directly without validation.
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

        var args = new OrderLinkRequestEventArgs(this, request) { ResponseCallback = ResponseHandler };
        listeners.Invoke(this, args);

        // Await the response if it was scheduled asynchronously.
        if (args.ResponseCallback != null && finalContext == null)
            await Task.Yield();

        if (finalContext == null)
        {
            // No response delivered: treat as failure.
            return;
        }

        if (finalContext.HasErrors)
            return;

        // Optionally: enforce requirement fulfillment here. For the skeleton, requirements
        // are surfaced via the response and assumed to be handled by the caller.

        ApplyLink(request, finalContext, createReferenceLocally: false, providedReference: finalReference);
    }

    /// <summary>
    /// Applies the linking decision to this container and raises <see cref="OrderLinkApplied"/>.
    /// </summary>
    protected void ApplyLink(
        OrderLinkingRequest request,
        ValidationContext context,
        bool createReferenceLocally,
        OrderReference? providedReference = null)
    {
        OrderReference? applied = null;

        if (request.IsUnlink)
        {
            _linkedOrder = null;
        }
        else
        {
            applied = providedReference
                ?? (createReferenceLocally && request.OrderNumber != null
                    ? new OrderReference(request.OrderNumber, request.OperationNumber)
                    : null);

            if (applied != null)
                _linkedOrder = applied;
        }

        RaiseResourceChanged();

        OrderLinkApplied?.Invoke(this, new OrderLinkAppliedEventArgs(this, request, context, applied));
    }

    /// <inheritdoc />
    public event EventHandler<OrderLinkRequestEventArgs>? OrderLinkRequested;

    /// <inheritdoc />
    public event EventHandler<OrderLinkAppliedEventArgs>? OrderLinkApplied;
}