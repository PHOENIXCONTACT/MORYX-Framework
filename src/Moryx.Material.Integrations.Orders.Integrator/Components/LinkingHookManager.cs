// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.Linking;
using Moryx.Orders;
using ValidationContext = Moryx.Material.Linking.ValidationContext;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(ILinkingHookManager))]
internal class LinkingHookManager : ILinkingHookManager, ILoggingComponent
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IModuleLogger Logger { get; set; }

    public IOrderManagement OrderManagement { get; set; }

    public IOrderLinkingHookFactory HookFactory { get; set; }

    public IOrderReferencesPool ReferencesPool { get; set; }

    public ModuleConfig Config { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void ProcessLinkingRequested(RequestContext context)
    {
        // TODO: Do we want this? If yes we need cancellation token/module stop handling
        // Schedule async handling without blocking the event source.
        _ = HandleLinkRequestedAsync(context);
    }

    private async Task HandleLinkRequestedAsync(RequestContext context)
    {
        var request = context.OrderRequest;
        var container = context.Container;
        var validation = new ValidationContext();
        Order? orderToBeLinked = null;
        OrderReference? referenceToBeLinked = null;

        try
        {
            // Resolve the new orderToBeLinked business object if this is a (re)link
            if (!request.IsUnlink && request.OrderNumber is not null)
            {
                orderToBeLinked = await OrderManagement.LoadOrderFor(request.OrderNumber, request.OperationNumber);
                referenceToBeLinked = ReferencesPool.GetOrCreate(request.OrderNumber, request.OperationNumber);
                if (orderToBeLinked == null)
                {
                    validation.AddError($"Order '{request.OrderNumber}' (operation '{request.OperationNumber}') could not be resolved. Hooks executed ");
                }
            }

            var previouslyLinkedOrder = await OrderManagement.LoadOrderFor(request.PreviousOrder);
            // Execute all configured hooks

            Config.Hooks.ForEach(async hook => await ExecuteHook(hook, container, request, validation, orderToBeLinked, previouslyLinkedOrder, false));
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error during linking request handling for container {id} - {name}", container.Id, container.Name);
            validation.AddError(ex);
        }

        if (context.ResponseCallback is null)
        {
            Logger?.LogWarning("Material container {id} - {name} requests linking to an order without providing a {callback}",
                container.Id, container.Name, nameof(LinkingRequestEventArgs.ResponseCallback));
            return;
        }

        // Deliver response to container
        var response = new LinkingResponse(validation, referenceToBeLinked);
        try
        {
            await context.ResponseCallback(response);
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Error, ex, "Container failed to handle linking response for {id} - {name}", container.Id, container.Name);
        }
    }

    private async Task ExecuteHook(OrderLinkingHookConfig hookName, IMaterialContainer container,
        OrderLinkingRequest request, ValidationContext validation, Order? order, Order? previousOrder, bool applyPhase)
    {
        OrderLinkingHook? hook = null;
        try
        {
            hook = await HookFactory.Create(hookName, CancellationToken.None) as OrderLinkingHook;
            if (hook is null)
            {
                validation.AddError($"Configured hook '{hookName}' could not be created.");
                return;
            }

            hook.Container = container;
            hook.Request = request;
            hook.ValidationContext = validation;
            hook.Order = order;
            hook.PreviousOrder = previousOrder;
            
            if (applyPhase)
            {
                await hook.HandleLinkAppliedAsync(CancellationToken.None);
            }
            else
            {
                await hook.HandleLinkRequestAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Error, ex, "Hook {0} threw during {1}", hookName, applyPhase ? "apply" : "request");
            validation.AddError(ex, hook?.GetType() ?? typeof(ILinkingHook));
        }
        finally
        {
            if (hook != null)
            {
                HookFactory.Destroy(hook);
            }
        }
    }

    public void ProcessLinkingApplied(AppliedContext context)
    {
        // TODO: Do we want this? If yes we need cancellation token/module stop handling
        _ = HandleLinkAppliedAsync(context);
    }

    private async Task HandleLinkAppliedAsync(AppliedContext context)
    {
        var request = context.OrderRequest;
        var container = context.Container;
        var validation = context.Validation;
        var hasLinkedOrder = !request.IsUnlink && request.OrderNumber is not null;
        var linkedReference = hasLinkedOrder ? ReferencesPool.GetOrCreate(request.OrderNumber, request.OperationNumber) : null;
        var linkedOrder = hasLinkedOrder ? await OrderManagement.LoadOrderFor(request.OrderNumber, request.OperationNumber) : null;
        var previouslyLinkedOrder = await OrderManagement.LoadOrderFor(request.PreviousOrder);
        try
        {
            Config.Hooks.ForEach(async hook =>
            await ExecuteHook(hook, container, request, validation, linkedOrder, previouslyLinkedOrder, false));
            //await RecordLineageAsync(e, request);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error during link applied handling for container {id} - {name}", container.Id, container.Name);
        }
    }

    //private async Task RecordLineageAsync(OrderLinkAppliedEventArgs e, OrderLinkingRequest request)
    //{
    //    var successful = !e.Context.HasErrors;

    //    if (request.PreviousOrder != null)
    //    {
    //        await MaterialManagement.RecordLineageAsync(new OrderUnlinkLineageEvent
    //        {
    //            ContainerId = e.Container.Id,
    //            OrderNumber = request.PreviousOrder.OrderNumber,
    //            OperationNumber = request.PreviousOrder.OperationNumber,
    //            Successful = successful,
    //            Description = "Order unlinked from container."
    //        });
    //    }

    //    if (!request.IsUnlink && e.AppliedReference != null)
    //    {
    //        await MaterialManagement.RecordLineageAsync(new OrderLinkLineageEvent
    //        {
    //            ContainerId = e.Container.Id,
    //            OrderNumber = e.AppliedReference.OrderNumber,
    //            OperationNumber = e.AppliedReference.OperationNumber,
    //            Successful = successful,
    //            Description = "Order linked to container."
    //        });
    //    }
    //}
}
