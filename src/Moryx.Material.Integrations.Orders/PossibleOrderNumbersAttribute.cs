// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Orders;

// ToDo: Move to orders namespace
public class PossibleOrderNumbersAttribute : PossibleValuesAttribute
{
    public override bool OverridesConversion => false;

    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(Container.IContainer container, IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IOrderManagement)) is not IOrderManagement facade)
        {
            return [];
        }

        var orders = Array.Empty<string>();
        try
        {
            return facade.GetOperations(o => o.State < OperationStateClassification.Completed)
                .Select(o => o.Order.Number).Distinct().ToArray();
        }
        // ToDo: Can we move HealthStateException to Moryx namespace? Currently this introduces dependency to Moryx.Runtime
        catch (HealthStateException)
        {
            return orders;
        }
    }
}
