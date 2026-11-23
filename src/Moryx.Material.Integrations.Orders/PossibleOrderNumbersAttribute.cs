// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Orders;

// ToDo: Move to orders namespace
/// <summary>
/// <see cref="PossibleValuesAttribute"/> resolving available <see cref="Order.Number"/>s from the
/// <see cref="IOrderManagement"/> facade.
/// </summary>

public class PossibleOrderNumbersAttribute : PossibleValuesAttribute
{
    /// <inheritdoc />
    public override bool OverridesConversion => false;

    /// <inheritdoc />
    public override bool UpdateFromPredecessor => false;

    /// <inheritdoc />
    public override IEnumerable<string>? GetValues(Container.IContainer container, IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IOrderManagement)) is not IOrderManagement facade)
        {
            throw new InvalidOperationException($"Tried to utilize {nameof(PossibleOrderNumbersAttribute)} without" +
                $" an {nameof(IOrderManagement)} facade being available. " +
                $"Make sure a corresponding module is present in your application.");
        }

        try
        {
            return facade.GetOperations(o => o.State < OperationStateClassification.Completed)
                .Select(o => o.Order.Number).Distinct().ToArray();
        }
        // ToDo: Can we move HealthStateException to Moryx namespace? Currently this introduces dependency to Moryx.Runtime
        catch (HealthStateException e)
        {
            throw new InvalidOperationException($"Tried to utilize {nameof(PossibleOrderNumbersAttribute)} while" +
                $" the {nameof(IOrderManagement)} module was not running. Check the modules state and retry.", e);
        }
    }
}
