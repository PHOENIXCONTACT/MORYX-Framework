// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;
using Moryx.Material.Linking.Integration;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(IOrderContainerManager))]
internal class OrderContainerManager : LinkedContainerManagerBase<OrderReference>, IOrderContainerManager
{
    // TODO: Move to ctor injection in this module to remove nullable warnings on required dependencies
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IOrderLinkHandler LinkHandler { get; set; }

    public IOrderReferencesPool ReferencesPool { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    /// <inheritdoc/>
    protected override ILinkHandler<OrderReference> Handler => LinkHandler;

    /// <inheritdoc/>
    protected override IReferencePool<OrderReference> Pool => ReferencesPool;

    /// <inheritdoc/>
    protected override MaterialContainerLink<OrderReference>? GetLink(IMaterialContainer container)
        => (container as IOrderLinkedMaterialContainer)?.OrderLink;
}
