// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;
using Moryx.Material.Linking.Integration;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(IProductContainerManager))]
internal class ProductContainerManager : LinkedContainerManagerBase<ProductTypeReference>, IProductContainerManager
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IProductLinkHandler LinkHandler { get; set; }

    public IProductTypeReferencesPool ReferencesPool { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    protected override ILinkHandler<ProductTypeReference> Handler => LinkHandler;

    protected override IReferencePool<ProductTypeReference> Pool => ReferencesPool;

    protected override MaterialContainerLink<ProductTypeReference>? GetLink(IMaterialContainer container)
        => (container as IProductLinkedMaterialContainer)?.ProductLink;
}
