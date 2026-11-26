// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.States;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Products;

/// <summary>
/// Default base class for <see cref="IProductLinkedMaterialContainer"/> implementations.
/// </summary>
/// <remarks>
/// Containers linked to further business objects do not need to derive from this class. They can implement
/// <see cref="IProductLinkedMaterialContainer"/> alongside other linked container interfaces by declaring a
/// <see cref="Products.ProductLink"/> property.
/// </remarks>
[DataContract]
public abstract class ProductLinkedMaterialContainer : MaterialContainer, IProductLinkedMaterialContainer
{
    /// <inheritdoc />
    [DataMember]
    public ProductLink ProductLink { get; } = new();

    /// <summary>
    /// Resource constructor method bringing a <see cref="MaterialContainer"/> from the <see cref="StateClassification.Uninitialized"/>
    /// state into the <see cref="StateClassification.Available"/> state using provided information and requesting to link it to
    /// a product right away. The link request stays pending until the product integration attaches to the container.
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
        return ProductLink.LinkAsync(productIdentity);
    }
}
