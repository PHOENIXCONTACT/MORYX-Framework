// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Microsoft.Extensions.DependencyInjection;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Maintenance.Endpoints.Mappers;
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Endpoints;

/// <summary>
/// Attribute to select maintainable resources  
/// </summary>
internal sealed class MaintainableResourcesAttribute : PossibleValuesAttribute
{
    public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
    {
        var values = GetPossibleValues(serviceProvider).Select(x => x.Name);
        return values;
    }

    /// <inheritdoc />
    public override bool OverridesConversion => true;

    /// <inheritdoc />
    public override bool UpdateFromPredecessor => true;

    /// <inheritdoc />
    public override object Parse(IContainer container, IServiceProvider serviceProvider, string value)
    {
        var possibleTypes = GetPossibleValues(serviceProvider);
        return possibleTypes.First(type => type.Name == value).Name;
    }

    private static IEnumerable<ResourceModel> GetPossibleValues(IServiceProvider serviceProvider)
    {
        return serviceProvider?
            .GetService<IResourceManagement>()?
            .GetResources<IMaintainableResource>()
            .Select(x => x.ToDto())
            ?? [];
    }
}
