// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.Facade;
using Moryx.Material.States;
using Moryx.Tools;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(IProductContainerManager))]
internal class ProductContainerManager : IProductContainerManager, ILoggingComponent
{
    private readonly ConcurrentDictionary<long, IProductLinkedMaterialContainer> _containers = [];

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IModuleLogger Logger { get; set; }

    public IMaterialManagement MaterialManagement { get; set; }

    public ILinkingHookManager HookManager { get; set; }

    public IProductTypeReferencesPool ProductReferencePool { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    #region Lifecycle

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        MaterialManagement.ContainerStateChanged += OnContainerStateChanged;

        var containers = MaterialManagement
            .GetContainers(c => c is IProductLinkedMaterialContainer { State: not StateClassification.Deregistered })
            .Cast<IProductLinkedMaterialContainer>();

        foreach (var container in containers)
        {
            await TryAddAsync(container, cancellationToken);
        }
    }

    private void OnContainerStateChanged(object? sender, ContainerStateChangedEventArgs e)
    {
        if (e.Container is not IProductLinkedMaterialContainer container)
        {
            return;
        }
        else if (e.NewStateInformation is DeregisteredStateInformation)
        {
            if (_containers.TryRemove(container.Id, out var removedContainer))
            {
                Detach(removedContainer);
                // TODO: Lineage related handling - record an unlink lineage event when a product-linked container is removed.
                //HandleContainerRemoved(removedContainer);
            }
        }
        else
        {
            // Fire-and-forget: the event source should not be blocked while we resolve the
            // container's linked product against the pool/facade.
            _ = TryAddAsync(container, CancellationToken.None);
        }
    }

    private async Task TryAddAsync(IProductLinkedMaterialContainer c, CancellationToken cancellationToken)
    {
        if (!_containers.TryAdd(c.Id, c))
        {
            return;
        }

        await SubstituteReferenceOfAsync(c, cancellationToken);
        c.ProductLinkRequested += OnProductLinkRequested;
        c.ProductLinkApplied += OnProductLinkApplied;
    }

    private void OnProductLinkRequested(object? sender, ProductLinkRequestEventArgs e)
    {
        if (sender is IProductLinkedMaterialContainer c)
        {
            HookManager.ProcessLinkingRequested(new(c, e));
        }
    }

    private void OnProductLinkApplied(object? sender, ProductLinkAppliedEventArgs e)
    {
        if (sender is not IProductLinkedMaterialContainer c)
        {
            return;
        }

        // Update pool bookkeeping: the container has switched from its previous product
        // reference to a new one (or to none, in case of an unlink).
        _ = UpdatePoolUsageAsync(e.ProductRequest.PreviousProduct, e.AppliedReference, CancellationToken.None);

        HookManager.ProcessLinkingApplied(new(c, e));
    }

    private async Task UpdatePoolUsageAsync(ProductTypeReference? previous, ProductTypeReference? applied, CancellationToken cancellationToken)
    {
        // Ignore no-op transitions (same identity before and after).
        if (previous.ValueEquals(applied))
        {
            return;
        }

        if (applied is not null)
        {
            await ProductReferencePool.AcquireAsync(applied, cancellationToken);
        }

        ProductReferencePool.Release(previous);
    }

    private async Task SubstituteReferenceOfAsync(IProductLinkedMaterialContainer value, CancellationToken cancellationToken)
    {
        var link = value.LinkedProductType;

        // Container is not linked.
        if (link is null)
        {
            return;
        }

        // Acquire (or create) the managed reference for the container's linked identity.
        // This checks existing pool entries first before falling back to the product facade.
        var managed = await ProductReferencePool.AcquireAsync(link, cancellationToken);

        // Link is already an actively managed reference: nothing to do.
        if (ReferenceEquals(managed, link))
        {
            return;
        }

        value.LinkedProductType = managed;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        MaterialManagement.ContainerStateChanged -= OnContainerStateChanged;
        foreach (var container in _containers.Values)
        {
            Detach(container);
        }
        _containers.Clear();

        return Task.CompletedTask;
    }

    private void Detach(IProductLinkedMaterialContainer container)
    {
        container.ProductLinkRequested -= OnProductLinkRequested;
        container.ProductLinkApplied -= OnProductLinkApplied;

        // Release our hold on the pooled reference. When no other container references the
        // same product identity anymore, the pool will drop the entry.
        ProductReferencePool.Release(container.LinkedProductType);
    }

    // TODO: Lineage related handling
    //private void HandleContainerRemoved(IProductLinkedMaterialContainer container)
    //{
    //    // Cascade auto-unlink: record an unlink lineage event when a product-linked container is removed.
    //    var reference = container.LinkedProductType;
    //    if (reference == null)
    //        return;
    //
    //    try
    //    {
    //        _ = MaterialManagement.RecordLineageAsync(new ProductTypeUnlinkLineageEvent
    //        {
    //            ContainerId = container.Id,
    //            ProductIdentity = reference.ProductIdentity,
    //            Successful = true,
    //            Description = "Product unlinked due to container removal."
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger?.Log(LogLevel.Warning, ex, "Failed to cascade unlink for container {0}", container.Id);
    //    }
    //}

    #endregion
}