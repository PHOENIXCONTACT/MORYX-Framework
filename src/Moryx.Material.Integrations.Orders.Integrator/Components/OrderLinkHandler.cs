// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Material.Linking;
using Moryx.Material.Linking.Integration;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(IOrderLinkHandler))]
internal class OrderLinkHandler : LinkHandlerBase<OrderReference, Order, OrderLinkingHookConfig>, IOrderLinkHandler
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IOrderManagement OrderManagement { get; set; }

    public IOrderLinkingHookFactory HookFactory { get; set; }

    public IOrderReferencesPool ReferencesPool { get; set; }

    public ModuleConfig Config { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    /// <inheritdoc/>
    protected override IReferencePool<OrderReference> Pool => ReferencesPool;

    /// <inheritdoc/>
    protected override IEnumerable<OrderLinkingHookConfig> HookConfigs => Config.Hooks;

    /// <inheritdoc/>
    protected override Task<ILinkingHook> CreateHookAsync(OrderLinkingHookConfig config, CancellationToken cancellationToken)
        => HookFactory.Create(config, cancellationToken);

    /// <inheritdoc/>
    protected override void DestroyHook(ILinkingHook hook) => HookFactory.Destroy(hook);

    /// <inheritdoc/>
    protected override async Task<(OrderReference Reference, Order? BusinessObject)> ResolveAsync(OrderReference target,
        CancellationToken cancellationToken)
    {
        var order = await LoadAsync(target, cancellationToken);
        return (ReferencesPool.GetOrCreate(target.OrderNumber, target.OperationNumber), order);
    }

    /// <inheritdoc/>
    protected override Task<Order?> LoadAsync(OrderReference reference, CancellationToken cancellationToken)
        => OrderManagement.LoadOrderFor(reference);
}
