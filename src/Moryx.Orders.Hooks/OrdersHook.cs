// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Runtime.Hooks;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Modules.Hooks;
using Moryx.Tools;

namespace Moryx.Orders.Management.Hooks;

/// <summary>
/// Hook that can be used to automatically create orders on startup
/// </summary>
public sealed class OrdersHook : ModuleStartHook<IOrderManagement, OrdersHookConfig>
{
    /// <summary>
    /// Construct the OrdersHook
    /// </summary>
    public OrdersHook(IModuleManager moduleManager, ILogger<OrdersHook> logger, IConfigManager configuration)
        : base(moduleManager, configuration, logger)
    {
        if (_config.Operations is not { Length: > 0 })
        {
            InitializationResult = FunctionResult.WithError("No operations defined");
        }
    }

    /// <inheritdoc />
    protected override async Task OnModuleStarted(IServerModule module, IOrderManagement facade)
    {
        var hasEntries = facade.GetOperations(o => true).Any();

        foreach (var operationDescription in _config.Operations!)
        {
            if (operationDescription.Disabled || (operationDescription.OnlyOnEmptyDb && hasEntries))
            {
                continue;
            }

            var context = CreateOperationsContext(operationDescription);
            await facade.AddOperationAsync(context);
        }
    }

    private static OperationCreationContext CreateOperationsContext(OrdersHookConfig.ImporterConfig operation)
    {
        return new()
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
