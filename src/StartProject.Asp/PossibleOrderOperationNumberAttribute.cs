// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Orders;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace StartProject.Asp;

public class PossibleOrderOperationNumberAttribute(bool showAdvisableOperationOnly = false) : PossibleValuesAttribute
{
    public override bool OverridesConversion => false;

    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(IContainer container, IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IOrderManagement)) is not IOrderManagement facade)
        {
            return [];
        }

        var orders = Array.Empty<string>();
        try
        {
            return [.. facade.GetOperations(o => showAdvisableOperationOnly == false || (showAdvisableOperationOnly == true && o.State.HasFlag(OperationStateClassification.CanAdvice)) )
            .SelectMany(o => o.Order.Operations.Select(op => ToDisplayString(o))).Distinct()];
        }
        catch (HealthStateException)
        {
            return orders;
        }
    }

    private static string ToDisplayString(Operation value)
    {
        return $"{value.Order.Number}-{value.Number} ({value.Product.Identity.Identifier} {value.Product.Name})";
    }

    public static string? GetOrderFrom(string value)
    {
        var splits = value.Split('-');
        return splits.Length > 0 ? splits[0].Trim(): null ;
    }

    public static string? GetOperationFrom(string value)
    {
        var result = value.Split('(').Take(1).First().Split('-').Last().Trim();
        return result;
    }
}
