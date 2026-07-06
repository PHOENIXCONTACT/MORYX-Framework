// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Runtime.Modules;

namespace Moryx.Orders.Management;

/// <summary>
/// Hook that can be used to automatically create orders on startup
/// </summary>
public sealed class OrdersLifecycleHook : ModuleLifecycleHookBase<IOrderManagement, OrdersLifecycleHookConfig>
{
    /// <inheritdoc />
    protected override ServerModuleState[] TargetStates => [ServerModuleState.Running];

    /// <summary>
    /// Construct the OrdersLifecycleHook
    /// </summary>
    public OrdersLifecycleHook(IModuleManager moduleManager, ILogger<OrdersLifecycleHook> logger, IConfigManager configuration)
        : base(moduleManager, configuration, logger)
    {
        if (Config.Operations is not { Length: > 0 })
        {
            SkipReason = "No operations defined";
        }
    }

    /// <inheritdoc />
    protected override async Task OnTargetStateReached(IServerModule module, IOrderManagement facade, ServerModuleState state)
    {
        var hasEntries = facade.GetOperations(_ => true).Any();

        foreach (var operationDescription in Config.Operations!)
        {
            if (operationDescription.Disabled || (operationDescription.OnlyOnEmptyDb && hasEntries))
            {
                continue;
            }

            var context = CreateOperationsContext(operationDescription);
            await facade.AddOperationAsync(context);
        }
    }

    private static OperationCreationContext CreateOperationsContext(OperationImportConfig operation)
    {
        return new OperationCreationContext
        {
            Number = operation.Number,
            Order = new OrderCreationContext()
            {
                Number = operation.OrderNumber ?? $"Order for {operation.ProductIdentifier}",
                Type = operation.OrderType,
            },
            ProductIdentifier = operation.ProductIdentifier,
            ProductRevision = operation.ProductRevision,
            Unit = operation.Unit,
            TotalAmount = operation.TotalAmount,
            UnderDeliveryAmount = operation.UnderDelivery,
            OverDeliveryAmount = operation.OverDelivery,
            Name = operation.Name,
            PlannedStart = DateTime.Now,
            PlannedEnd = DateTime.Now.AddDays(1),
            RecipePreselection = operation.RecipePreselection,
        };
    }
}
