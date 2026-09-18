// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material;
using Moryx.Material.Integrations.Orders;
using Moryx.Material.States;
using Moryx.Serialization;

namespace StartProject.Asp;

[Description("Container that is linked to an order and contains pick parts")]
public partial class PickPartMaterialContainer : OrderLinkedMaterialContainer
{

    public override void With([Display(Name = "Identity Kind", Description = "Type of identity for the Container (e.g. Serialnumber)"), PossibleTypes(typeof(IIdentity))] IIdentity identityType = null, [Display(Name = "Identity", Description = "Identity unique to the Container (e.g. 123-456-789)")] string identity = null, [Display(Name = "Material", Description = "The material in the container")] string material = null, [Display(Name = "Quantity", Description = "Amount of material in the container")] double quantity = 0, [Display(Name = "Unit", Description = "Unit the quantity is given in")] string unit = null)
    {
        base.With(identityType, identity, material, quantity, unit);
    }

    public override void With(
        [Display(Name = "Request Information", Description = "Specifications for the material request")] MaterialRequest request)
    {
        base.With(request);
    }

    public override Task With(
        [Display(Name = "Order Number", Description = "Order number this container is linked to"), PossibleOrderNumbers] string orderNumber,
        [Display(Name = "Operation Number", Description = "Operation number this container is linked to"), PossibleOperationNumbers] string operationNumber = null,
        [Display(Name = "Identity Kind", Description = "Type of identity for the Container (e.g. Serialnumber)"), PossibleTypes(typeof(IIdentity))] IIdentity identityType = null, [Display(Name = "Identity", Description = "Identity unique to the Container (e.g. 123-456-789)")] string identity = null, [Display(Name = "Material", Description = "The material in the container")] string material = null, [Display(Name = "Quantity", Description = "Amount of material in the container")] double quantity = 0, [Display(Name = "Unit", Description = "Unit the quantity is given in")] string unit = null)
    {
        return base.With(orderNumber, operationNumber, identityType, identity, material, quantity, unit);
    }

    [ResourceConstructor]
    [Display(Name = "Pick Part Container", Description = "Create a Pick Part material container that is linked to an order")]
    public Task ConstructWith([Display(Name = "Pick Part options"), PossiblePickPartModel] PickPartModel model)
    {
        StateInformation = new RequestedStateInformation();
        Identity = new BatchIdentity(model.Container);
        Material = model.Product;
        Unit = "pcs";
        return RequestOrderLinkAsync(model.Order, model.Operation);
    }
}
