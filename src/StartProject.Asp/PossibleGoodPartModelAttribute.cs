// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Orders;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace StartProject.Asp;

public class PossibleGoodPartModelAttribute : PossibleValuesAttribute
{
    public override bool OverridesConversion => true;

    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(Moryx.Container.IContainer localContainer, IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IOrderManagement)) is not IOrderManagement facade)
        {
            return [];
        }

        var orders = Array.Empty<string>();
        try
        {
            return [.. facade.GetOperations(o => o.State.HasFlag(OperationStateClassification.CanAdvice))
            .SelectMany(o => o.Order.Operations.Select(op => ToDisplayString(o))).Distinct()];
        }
        catch (HealthStateException)
        {
            return orders;
        }
    }

    public override object Parse(Moryx.Container.IContainer container, IServiceProvider serviceProvider, string value)
    {
        if (serviceProvider.GetService(typeof(IOrderManagement)) is not IOrderManagement orderFacade)
        {
            return new GoodPartModel();
        }

        var operationNumber = GetOperationFrom(value);
        var order = GetOrderFrom(value);
        var operation = orderFacade.GetOperations(x => x.Order.Number == order && x.Number == operationNumber).FirstOrDefault();
        var product = operation.Product.Identity is ProductIdentity identity ? identity.ToString() : string.Join('-', operation.Product.Identity.Identifier, "00") + $" {operation.Product.Name}";
        var model = new GoodPartModel
        {
            Order = order,
            Operation = operationNumber,
            Product = product,
        };
        return model;
    }

    private static string ToDisplayString(Operation value)
    {
        return $"{value.Order.Number}-{value.Number} ({value.Product.Identity.Identifier} {value.Product.Name})";
    }

    private static string? GetOrderFrom(string value)
    {
        var splits = value.Split('-');
        return splits.Length > 0 ? splits[0].Trim() : null;
    }

    private static string? GetOperationFrom(string value)
    {
        var result = value.Split('(').Take(1).First().Split('-').Last().Trim();
        return result;
    }
}

