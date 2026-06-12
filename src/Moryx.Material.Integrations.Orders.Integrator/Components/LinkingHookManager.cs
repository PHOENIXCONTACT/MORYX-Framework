// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.Linking;
using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(ILinkingHookManager))]
internal class LinkingHookManager : ILinkingHookManager, ILoggingComponent
{
    public IModuleLogger Logger { get; set; } = null!;

    public IResourceManagement ResourceManagement { get; set; } = null!;

    public IOrderManagement OrderManagement { get; set; } = null!;

    public IMaterialManagement MaterialManagement { get; set; } = null!;

    public ILinkingHookFactory HookFactory { get; set; } = null!;

    public ModuleConfig Config { get; set; } = null!;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ResourceManagement.ResourceAdded += OnResourceAdded;
        ResourceManagement.ResourceRemoved += OnResourceRemoved;

        foreach (var container in ResourceManagement.GetResources<IOrderLinkedMaterialContainer>())
            Subscribe(container);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ResourceManagement.ResourceAdded -= OnResourceAdded;
        ResourceManagement.ResourceRemoved -= OnResourceRemoved;

        foreach (var container in ResourceManagement.GetResources<IOrderLinkedMaterialContainer>())
            Unsubscribe(container);

        return Task.CompletedTask;
    }

    private void OnResourceAdded(object? sender, IResource resource)
    {
        if (resource is IOrderLinkedMaterialContainer container)
            Subscribe(container);
    }

    private void OnResourceRemoved(object? sender, IResource resource)
    {
        if (resource is IOrderLinkedMaterialContainer container)
            Unsubscribe(container);
    }

    private void Subscribe(IOrderLinkedMaterialContainer container)
    {
        container.OrderLinkRequested += OnOrderLinkRequested;
        container.OrderLinkApplied += OnOrderLinkApplied;
    }

    private void Unsubscribe(IOrderLinkedMaterialContainer container)
    {
        container.OrderLinkRequested -= OnOrderLinkRequested;
        container.OrderLinkApplied -= OnOrderLinkApplied;
    }

    private void OnOrderLinkRequested(object? sender, OrderLinkRequestEventArgs e)
    {
        // Schedule async handling without blocking the event source.
        _ = HandleLinkRequestedAsync(e);
    }

    private async Task HandleLinkRequestedAsync(OrderLinkRequestEventArgs e)
    {
        var request = e.OrderRequest;
        var validationContext = new ValidationContext();
        Order? order = null;
        var previousOrder = request.PreviousOrder?.Order;

        try
        {
            // Resolve the new order business object if this is a (re)link
            if (!request.IsUnlink && request.OrderNumber != null)
            {
                var operation = await OrderManagement
                    .LoadOperationAsync(request.OrderNumber, request.OperationNumber ?? string.Empty)
                    .ConfigureAwait(false);

                order = operation?.Order;
                if (order == null)
                {
                    validationContext.AddError(
                        $"Order '{request.OrderNumber}' (operation '{request.OperationNumber}') could not be resolved.",
                        GetType());
                }
                else
                {
                    var reference = new OrderReference(request.OrderNumber, request.OperationNumber);
                    reference.Attach(order);
                    request.NewOrder = reference;
                }
            }

            // Execute all configured hooks
            await ExecuteHooksAsync(
                e,
                validationContext,
                order,
                previousOrder,
                applyPhase: false,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Error, ex, "Error during link request handling for container {0}", e.Container.Id);
            validationContext.AddError(ex, GetType());
        }

        // Deliver response to container
        var response = new LinkingResponse(validationContext, request.NewOrder);
        if (e.ResponseCallback != null)
        {
            try
            {
                await e.ResponseCallback(response);
            }
            catch (Exception ex)
            {
                Logger?.Log(LogLevel.Error, ex, "Container failed to handle linking response for {0}", e.Container.Id);
            }
        }
    }

    private void OnOrderLinkApplied(object? sender, OrderLinkAppliedEventArgs e)
    {
        _ = HandleLinkAppliedAsync(e);
    }

    private async Task HandleLinkAppliedAsync(OrderLinkAppliedEventArgs e)
    {
        var request = (OrderLinkingRequest)e.Request;
        var order = e.AppliedReference?.Order ?? request.NewOrder?.Order;
        var previousOrder = request.PreviousOrder?.Order;

        try
        {
            // Forge a synthetic OrderLinkRequestEventArgs for hook context propagation
            var requestArgs = new OrderLinkRequestEventArgs((IOrderLinkedMaterialContainer)e.Container, request);
            await ExecuteHooksAsync(
                requestArgs,
                e.Context,
                order,
                previousOrder,
                applyPhase: true,
                CancellationToken.None);

            await RecordLineageAsync(e, request);
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Error, ex, "Error during link applied handling for container {0}", e.Container.Id);
        }
    }

    private async Task ExecuteHooksAsync(
        OrderLinkRequestEventArgs e,
        ValidationContext context,
        Order? order,
        Order? previousOrder,
        bool applyPhase,
        CancellationToken cancellationToken)
    {
        foreach (var hookName in Config.Hooks)
        {
            LinkingHook? hook = null;
            try
            {
                hook = HookFactory.Create(hookName);
                if (hook == null)
                {
                    context.AddError($"Configured hook '{hookName}' could not be created.", GetType());
                    continue;
                }

                hook.Container = e.Container;
                hook.Request = e.Request;
                hook.ValidationContext = context;

                if (hook is OrderLinkingHook orderHook)
                {
                    orderHook.Order = order;
                    orderHook.PreviousOrder = previousOrder;
                }

                if (applyPhase)
                    await hook.HandleLinkAppliedAsync(cancellationToken);
                else
                    await hook.HandleLinkRequestAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.Log(LogLevel.Error, ex, "Hook {0} threw during {1}", hookName, applyPhase ? "apply" : "request");
                context.AddError(ex, hook?.GetType() ?? typeof(LinkingHook));
            }
            finally
            {
                if (hook != null)
                    HookFactory.Destroy(hook);
            }
        }
    }

    private async Task RecordLineageAsync(OrderLinkAppliedEventArgs e, OrderLinkingRequest request)
    {
        var successful = !e.Context.HasErrors;

        if (request.PreviousOrder != null)
        {
            await MaterialManagement.RecordLineageAsync(new OrderUnlinkLineageEvent
            {
                ContainerId = e.Container.Id,
                OrderNumber = request.PreviousOrder.OrderNumber,
                OperationNumber = request.PreviousOrder.OperationNumber,
                Successful = successful,
                Description = "Order unlinked from container."
            });
        }

        if (!request.IsUnlink && e.AppliedReference != null)
        {
            await MaterialManagement.RecordLineageAsync(new OrderLinkLineageEvent
            {
                ContainerId = e.Container.Id,
                OrderNumber = e.AppliedReference.OrderNumber,
                OperationNumber = e.AppliedReference.OperationNumber,
                Successful = successful,
                Description = "Order linked to container."
            });
        }
    }
}
