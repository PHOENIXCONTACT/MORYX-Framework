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

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Default base class for <see cref="IProductLinkedMaterialContainer"/> implementations.
/// </summary>
[DataContract]
public abstract class ProductLinkedMaterialContainer : MaterialContainer, IProductLinkedMaterialContainer
{
    #region IProductLinkedMaterialContainer
    private ProductTypeReference? _linkedProductType;

    /// <inheritdoc />
    [DataMember]
    public ProductTypeReference? LinkedProductType
    {
        get => _linkedProductType;
        set
        {
            var previous = _linkedProductType;
            _linkedProductType = value;
            if (!previous.ValueEquals(value))
            {
                RaiseResourceChanged();
            }
        }
    }

    /// <inheritdoc />
    public virtual async Task RequestProductLinkAsync(string productIdentity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(productIdentity))
        {
            throw new ArgumentException("Product identity is required.", nameof(productIdentity));
        }

        var request = new ProductLinkingRequest(productIdentity, _linkedProductType);
        await ExecuteLinkingProtocolAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task RequestProductUnlinkAsync(CancellationToken cancellationToken = default)
    {
        if (_linkedProductType == null)
        {
            return;
        }

        var request = new ProductLinkingRequest(_linkedProductType);
        await ExecuteLinkingProtocolAsync(request, cancellationToken);
    }

    /// <summary>
    /// Drives the two-phase linking protocol against any registered listeners
    /// (typically the <c>LinkingHookManager</c>).
    /// </summary>
    protected virtual async Task ExecuteLinkingProtocolAsync(ProductLinkingRequest request, CancellationToken cancellationToken)
    {
        var listeners = ProductLinkRequested;
        if (listeners is null)
        {
            // No integration is currently active.
            // We "fail" gracefully in this case and apply the reference directly without validation. While this
            // could circumvent validation hooks in certain cases, it prevents process interruptions and even allows
            // for using product-linked material containers without the sophisticated integration infrastructure.
            ApplyLink(request, new ValidationContext(), createReferenceLocally: true);
            return;
        }

        ValidationContext? finalContext = null;
        ProductTypeReference? finalReference = null;

        Task ResponseHandler(LinkingResponse response)
        {
            finalContext = response.Context;
            finalReference = response.Reference as ProductTypeReference;
            return Task.CompletedTask;
        }

        var args = new ProductLinkRequestEventArgs(request, ResponseHandler);
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
    /// Applies the linking decision to this container and raises <see cref="ProductLinkApplied"/>.
    /// </summary>
    protected void ApplyLink(ProductLinkingRequest request, ValidationContext context, bool createReferenceLocally, ProductTypeReference? providedReference = null)
    {
        ProductTypeReference? applied = null;

        if (request.IsUnlink)
        {
            applied = _linkedProductType = null;
        }
        else if (providedReference is not null)
        {
            applied = _linkedProductType = providedReference;
        }
        else if (createReferenceLocally && !string.IsNullOrEmpty(request.ProductIdentity))
        {
            applied = _linkedProductType = new ProductTypeReference(request.ProductIdentity);
        }
        else
        {
            throw new InvalidOperationException($"Cannot apply {nameof(ProductTypeReference)} " +
                $"{(providedReference is null ? "" : providedReference.ProductIdentity)} " +
                $"for {nameof(ProductLinkingRequest)} {request.ProductIdentity}");
        }

        RaiseResourceChanged();

        ProductLinkApplied?.Invoke(this, new ProductLinkAppliedEventArgs(request, context, applied));
    }

    /// <summary>
    /// Hook allowing subclasses to react to validation errors returned during linking.
    /// </summary>
    protected virtual Task HandleValidationErrors(ValidationContext? context) => Task.CompletedTask;

    /// <inheritdoc />
    public event EventHandler<ProductLinkRequestEventArgs>? ProductLinkRequested;

    /// <inheritdoc />
    public event EventHandler<ProductLinkAppliedEventArgs>? ProductLinkApplied;
    #endregion

    /// <summary>
    /// Resource constructor method bringing a <see cref="MaterialContainer"/> from the <see cref="StateClassification.Uninitialized"/>
    /// state into the <see cref="StateClassification.Available"/> state using provided information and requesting to link it to
    /// a product right away.
    /// </summary>
    /// <param name="productIdentity">Identity of the product to link.</param>
    /// <param name="identityType">Optional identity instance used to represent the container identity.</param>
    /// <param name="identity">Optional identifier value assigned to <paramref name="identityType"/>.</param>
    /// <param name="material">Optional material reference contained in the container.</param>
    /// <param name="quantity">Initial quantity contained in the container.</param>
    /// <param name="unit">Optional unit of <paramref name="quantity"/>.</param>
    [ResourceConstructor]
    [Display(Name = "Material Registration", Description = "Create a material container that is linked to a product")]
    public virtual Task With(
        [Display(Name = "Product Identity", Description = "Product identity this container is linked to"), PossibleProductIdentities] string productIdentity,
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
        return RequestProductLinkAsync(productIdentity);
    }
}