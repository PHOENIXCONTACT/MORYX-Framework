// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material;
using Moryx.Material.Integrations.Orders;
using Moryx.Material.States;
using Moryx.Orders;
using Moryx.Serialization;

namespace StartProject.Asp;

[Description("Container that is linked to an order and contains Good parts")]
public class GoodPartContainer : OrderLinkedMaterialContainer
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
    [Display(Name = "Good Part Container", Description = "Create a Good Part material container that is linked to an order")]
    public Task ConstructWith(
        [Display(Name = "Order-Operation Number", Description = "Order-Operation number this container is linked to"), PossibleOrderOperationNumber(showAdvisableOperationOnly: true)] string orderOperation,
        [ReadOnly(true), Display(Name = "Material")] string material,
        [Display(Name = "Container Identity")] string containerIdentity,
        [Display(Name = "Amount")] int amount
        )
    {
        StateInformation = new RequestedStateInformation();
        Identity = new BatchIdentity(containerIdentity);
        Material = material;
        Unit = "pcs";
        Quantity = amount;
        var operation = PossibleOrderOperationNumberAttribute.GetOperationFrom(orderOperation);
        var order = PossibleOrderOperationNumberAttribute.GetOrderFrom(orderOperation);
        return RequestOrderLinkAsync(order!, operation);
    }

    public override Task UpdateAsync(Dictionary<string, Entry> entries, Func<Type, object> getDependency, CancellationToken ct)
    {
        var orderManager = getDependency(typeof(IOrderManagement));
        if (orderManager is not IOrderManagement orderFacade || (!entries.TryGetValue("orderOperation", out var orderOperation) && !entries.TryGetValue("material", out var material) && material.Value.Current == Material))
        {
            return Task.CompletedTask;
        }

        var operationNumber = PossibleOrderOperationNumberAttribute.GetOperationFrom(orderOperation.Value.Current);
        var order = PossibleOrderOperationNumberAttribute.GetOrderFrom(orderOperation.Value.Current);

        var operations = orderFacade.GetOperations(x => x.Order.Number == order && x.Number == operationNumber);
        var operation = operations.FirstOrDefault();
        entries["material"]?.Value.Current = operation?.Product.Name;
        entries["material"]?.Value.Default = operation?.Product.Name;
        return Task.CompletedTask;
    }
}
