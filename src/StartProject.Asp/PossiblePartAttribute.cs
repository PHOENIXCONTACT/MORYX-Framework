// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace StartProject.Asp;

// ToDo: Move to products namespace
/// <summary>
/// <see cref="PossibleValuesAttribute"/> resolving available product identities from the
/// </summary>
public class PossiblePartAttribute : PossibleValuesAttribute
{
    /// <inheritdoc />
    public override bool OverridesConversion => false;

    /// <inheritdoc />
    public override bool UpdateFromPredecessor => false;

    /// <inheritdoc />
    public override IEnumerable<string> GetValues(IContainer container, IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IProductManagement)) is not IProductManagement facade)
        {
            return [];
        }

        var empty = Array.Empty<string>();
        try
        {
            var types = facade.LoadTypesAsync(new ProductQuery()).GetAwaiter().GetResult();
            return types.Select(t => t.Name).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().Distinct().ToArray();
        }
        catch (HealthStateException)
        {
            return empty;
        }
    }

    public static string GetProductNameFrom(string value)
        => value;
}
