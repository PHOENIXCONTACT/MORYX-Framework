// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Material.IntegrationTests;

/// <summary>
/// Test resource that hosts <see cref="MaterialContainer"/> instances. Mimics the
/// scenarios where a resource takes an action within the material-flow in MORYX (e.g.
/// registers a new <see cref="MaterialContainer"/> or material via a <see cref="MaterialRequest"/>
/// using the <see cref="IResourceGraph"/>.
/// </summary>
[DataContract]
[ResourceRegistration]
public class TestContainerHost : Resource
{
    /// <summary>
    /// Should create a <see cref="BasicMaterialContainer"/> in <see cref="States.StateClassification.Available"/>
    /// using the resource constructor for material registration.
    /// </summary>
    public async Task<MaterialContainer> RegisterMaterialAsync(string material, double quantity, string? unit, CancellationToken cancellationToken = default)
    {
        var container = Graph.Instantiate<BasicMaterialContainer>().Configure(c => c.With(
            material: material,
            quantity: quantity,
            unit: unit));

        await Graph.SaveAsync(container, cancellationToken);
        return container;
    }

    /// <summary>
    /// Should create a <see cref="BasicMaterialContainer"/> in <see cref="States.StateClassification.Requested"/>
    /// using the resource constructor for a <see cref="MaterialRequest"/>.
    /// </summary>
    public async Task<MaterialContainer> RequestMaterialAsync(MaterialRequest request, CancellationToken cancellationToken = default)
    {
        var container = Graph.Instantiate<BasicMaterialContainer>().Configure(c => c.With(request));

        await Graph.SaveAsync(container, cancellationToken);
        return container;
    }
}
