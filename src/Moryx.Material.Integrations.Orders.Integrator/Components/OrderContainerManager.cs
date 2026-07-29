// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.Facade;
using Moryx.Material.States;
using Moryx.Tools;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(IOrderContainerManager))]
internal class OrderContainerManager : IOrderContainerManager, ILoggingComponent
{
    private readonly ConcurrentDictionary<long, IOrderLinkedMaterialContainer> _containers = [];

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IModuleLogger Logger { get; set; }

    public IMaterialManagement MaterialManagement { get; set; }

    public ILinkingHookManager HookManager { get; set; }

    public IOrderReferencesPool OperationReferencePool { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    #region Lifecycle

    public Task StartAsync(CancellationToken cancellationToken)
    {
        MaterialManagement.ContainerStateChanged += OnContainerStateChanged;
        MaterialManagement.GetContainers(c => c is IOrderLinkedMaterialContainer { State: not StateClassification.Deregistered })
            .Cast<IOrderLinkedMaterialContainer>().ForEach(TryAdd);
        _containers.ForEach(pair => SubstituteReferenceOf(pair.Value));

        return Task.CompletedTask;
    }

    private void OnContainerStateChanged(object? sender, ContainerStateChangedEventArgs e)
    {
        if (e.Container is not IOrderLinkedMaterialContainer container)
        {
            return;
        }
        else if (e.NewStateInformation is DeregisteredStateInformation)
        {
            if (_containers.TryRemove(container.Id, out var removedContainer))
            {
                Detach(removedContainer);
                //HandleContainerRemoved(removedContainer);
            }
        }
        else
        {
            TryAdd(container);
        }
    }

    private void TryAdd(IOrderLinkedMaterialContainer c)
    {
        if (!_containers.TryAdd(c.Id, c))
        {
            return;
        }

        SubstituteReferenceOf(c);
        c.OrderLinkRequested += OnOrderLinkRequested;
        c.OrderLinkApplied += OnOrderLinkApplied;
    }

    private void OnOrderLinkRequested(object? sender, OrderLinkRequestEventArgs e)
    {
        if (sender is IOrderLinkedMaterialContainer c)
        {
            HookManager.ProcessLinkingRequested(new(c, e));
        }
    }

    private void OnOrderLinkApplied(object? sender, OrderLinkAppliedEventArgs e)
    {
        if (sender is IOrderLinkedMaterialContainer c)
        {
            HookManager.ProcessLinkingApplied(new(c, e));
        }
    }

    private void SubstituteReferenceOf(IOrderLinkedMaterialContainer value)
    {
        var link = value.LinkedOrder;

        // Container is not linked
        if (link is null)
        {
            return;
        }

        var reference = OperationReferencePool.Get(link);
        // Link is already an actively managed reference
        if (ReferenceEquals(reference, link))
        {
            return;
        }

        // Link references an unknown order in the system
        if (reference is null)
        {
            value.LinkedOrder = OperationReferencePool.GetOrCreate(link.OrderNumber, link.OperationNumber);
            return;
        }

        // Link is updated and now actively managed
        value.LinkedOrder = reference;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        MaterialManagement.ContainerStateChanged -= OnContainerStateChanged;
        _containers.ForEach(Detach);
        _containers.Clear();

        return Task.CompletedTask;
    }

    private void Detach(KeyValuePair<long, IOrderLinkedMaterialContainer> pair)
    {
        Detach(pair.Value);
    }

    private void Detach(IOrderLinkedMaterialContainer container)
    {
        container.OrderLinkRequested -= OnOrderLinkRequested;
        container.OrderLinkApplied -= OnOrderLinkApplied;
    }

    // TODO: Lineage realted handling
    //private void HandleContainerRemoved(IOrderLinkedMaterialContainer container)
    //{
    //    // Cascade auto-unlink: record an unlink lineage event when an order-linked container is removed.
    //    var reference = container.LinkedOrder;
    //    if (reference == null)
    //        return;

    //    try
    //    {
    //        _ = MaterialManagement.RecordLineageAsync(new OrderUnlinkLineageEvent
    //        {
    //            ContainerId = container.Id,
    //            OrderNumber = reference.OrderNumber,
    //            OperationNumber = reference.OperationNumber,
    //            Successful = true,
    //            Description = "Order unlinked due to container removal."
    //        });

    //        reference.Detach();
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger?.Log(LogLevel.Warning, ex, "Failed to cascade unlink for container {0}", container.Id);
    //    }
    //}

    #endregion
}
