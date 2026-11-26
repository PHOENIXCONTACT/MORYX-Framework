// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Material.Linking;
using Moryx.Material.Linking.Integration;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

/// <summary>
/// Executes the linking protocol for all <see cref="ProductLink"/>s, orchestrating the configured
/// <see cref="ProductLinkingHook"/> plugins.
/// </summary>
internal interface IProductLinkHandler : ILinkHandler<ProductTypeReference>
{
}

[Component(LifeCycle.Singleton, typeof(IProductLinkHandler))]
internal class ProductLinkHandler : LinkHandlerBase<ProductTypeReference, ProductType, ProductLinkingHookConfig>, IProductLinkHandler
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IProductManagement ProductManagement { get; set; }

    public IProductLinkingHookFactory HookFactory { get; set; }

    public IProductTypeReferencesPool ReferencesPool { get; set; }

    public ModuleConfig Config { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    protected override IReferencePool<ProductTypeReference> Pool => ReferencesPool;

    protected override IEnumerable<ProductLinkingHookConfig> HookConfigs => Config.Hooks;

    protected override Task<ILinkingHook> CreateHookAsync(ProductLinkingHookConfig config, CancellationToken cancellationToken)
        => HookFactory.Create(config, cancellationToken);

    protected override void DestroyHook(ILinkingHook hook) => HookFactory.Destroy(hook);

    protected override async Task<(ProductTypeReference Reference, ProductType? BusinessObject)> ResolveAsync(ProductTypeReference target,
        CancellationToken cancellationToken)
    {
        // The pool checks its existing entries first and only queries the facade when necessary
        var reference = await ReferencesPool.ResolveAsync(target.ProductIdentity, cancellationToken);
        return (reference, reference.ProductType);
    }

    protected override Task<ProductType?> LoadAsync(ProductTypeReference reference, CancellationToken cancellationToken)
        => ProductManagement.LoadProductFor(reference);
}
