// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(IOrderContainerManager))]
internal class OrderContainerManager : IOrderContainerManager, ILoggingComponent
{
    public IModuleLogger Logger { get; set; } = null!;

    public IResourceManagement ResourceManagement { get; set; } = null!;

    public IOrderManagement OrderManagement { get; set; } = null!;

    public IMaterialManagement MaterialManagement { get; set; } = null!;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Restore references for already-loaded containers
        foreach (var container in ResourceManagement.GetResources<IOrderLinkedMaterialContainer>())
            ActivateReference(container);

        ResourceManagement.ResourceAdded += OnResourceAdded;
        ResourceManagement.ResourceRemoved += OnResourceRemoved;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ResourceManagement.ResourceAdded -= OnResourceAdded;
        ResourceManagement.ResourceRemoved -= OnResourceRemoved;

        // Deactivate all references on shutdown
        foreach (var container in ResourceManagement.GetResources<IOrderLinkedMaterialContainer>())
        {
            container.LinkedOrder?.Detach();
        }
        return Task.CompletedTask;
    }

    private void OnResourceAdded(object? sender, IResource resource)
    {
        if (resource is IOrderLinkedMaterialContainer container)
            ActivateReference(container);
    }

    private void OnResourceRemoved(object? sender, IResource resource)
    {
        if (resource is IOrderLinkedMaterialContainer container)
            HandleContainerRemoved(container);
    }

    private void ActivateReference(IOrderLinkedMaterialContainer container)
    {
        var reference = container.LinkedOrder;
        if (reference == null)
            return;

        try
        {
            // Best-effort synchronous resolution; LoadOperationAsync is async so we trigger a fire-and-forget
            _ = ResolveAsync(reference);
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Warning, ex, "Failed to activate reference for container {0}", container.Id);
            reference.MarkUnavailable();
        }
    }

    private async Task ResolveAsync(OrderReference reference)
    {
        var operation = await OrderManagement
            .LoadOperationAsync(reference.OrderNumber, reference.OperationNumber ?? string.Empty)
            .ConfigureAwait(false);

        if (operation?.Order != null)
            reference.Attach(operation.Order);
        else
            reference.MarkUnavailable();
    }

    private void HandleContainerRemoved(IOrderLinkedMaterialContainer container)
    {
        // Cascade auto-unlink: record an unlink lineage event when an order-linked container is removed.
        var reference = container.LinkedOrder;
        if (reference == null)
            return;

        try
        {
            _ = MaterialManagement.RecordLineageAsync(new OrderUnlinkLineageEvent
            {
                ContainerId = container.Id,
                OrderNumber = reference.OrderNumber,
                OperationNumber = reference.OperationNumber,
                Successful = true,
                Description = "Order unlinked due to container removal."
            });

            reference.Detach();
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Warning, ex, "Failed to cascade unlink for container {0}", container.Id);
        }
    }
}