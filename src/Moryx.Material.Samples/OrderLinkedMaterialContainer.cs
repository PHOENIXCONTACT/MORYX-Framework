// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Integrations.Orders;
using Moryx.Material.States;
using Moryx.Serialization;

namespace Moryx.Material.Samples;

/// <summary>
/// Sample implementation for an <see cref="IOrderLinkedMaterialContainer"/>.
/// </summary>
/// <remarks>
/// Containers linked to further business objects can implement <see cref="IOrderLinkedMaterialContainer"/> alongside
/// other linked container interfaces by declaring an <see cref="Integrations.Orders.OrderLink"/> property.
/// </remarks>
[DataContract]
public class OrderLinkedMaterialContainer : MaterialContainer, IOrderLinkedMaterialContainer
{
    /// <inheritdoc />
    [DataMember]
    public OrderLink OrderLink { get; } = new();

    /// <summary>
    /// Resource constructor method bringing a <see cref="MaterialContainer"/> from the <see cref="StateClassification.Uninitialized"/>
    /// state into the <see cref="StateClassification.Available"/> state using provided information and requesting to link it to
    /// an order right away. The link request stays pending until the order integration attaches to the container.
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
        return OrderLink.LinkAsync(orderNumber, operationNumber);
    }
}
