// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Orders;

// ToDo: Move to orders namespace
/// <summary>
/// <see cref="PossibleValuesAttribute"/> resolving available <see cref="Operation.Number"/>s from the
/// <see cref="IOrderManagement"/> facade.
/// </summary>
public class PossibleOperationNumbersAttribute : PossibleValuesAttribute
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
            throw new InvalidOperationException($"Tried to utilize {nameof(PossibleOperationNumbersAttribute)} without" +
                $" an {nameof(IOrderManagement)} facade being available. " +
                $"Make sure a corresponding module is present in your application.");
        }

        var orders = Array.Empty<string>();
        try
        {
            return facade.GetOperations(o => o.State < OperationStateClassification.Completed)
                .Select(o => o.Number).Distinct().ToArray();
        }
        catch (HealthStateException e)
        {
            throw new InvalidOperationException($"Tried to utilize {nameof(PossibleOperationNumbersAttribute)} while" +
                $" the {nameof(IOrderManagement)} module was not running. Check the modules state and retry.", e);
        }
    }
}
