// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Material.Integrations.Products;

// ToDo: Move to products namespace
/// <summary>
/// <see cref="PossibleValuesAttribute"/> resolving available product identities from the
/// <see cref="IProductManagement"/> facade.
/// </summary>
public class PossibleProductIdentitiesAttribute : PossibleValuesAttribute
{
    /// <inheritdoc />
    public override bool OverridesConversion => false;

    /// <inheritdoc />
    public override bool UpdateFromPredecessor => false;

    /// <inheritdoc />
    public override IEnumerable<string> GetValues(Container.IContainer container, IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IProductManagement)) is not IProductManagement facade)
        {
            return [];
        }

        var empty = Array.Empty<string>();
        try
        {
            var types = facade.LoadTypesAsync(new ProductQuery()).GetAwaiter().GetResult();
            return types.Select(t => t.Identity?.ToString()).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().Distinct().ToArray();
        }
        catch (HealthStateException)
        {
            return empty;
        }
    }
}