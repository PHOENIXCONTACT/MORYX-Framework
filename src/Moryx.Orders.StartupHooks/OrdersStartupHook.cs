// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Modules.StartupHooks;
using Moryx.Tools;

namespace Moryx.Orders.StartupHooks;

/// <summary>
/// Hook that can be used to automatically create orders on startup
/// </summary>
public sealed class OrdersStartupHook : ModuleStartupStartHookBase<IOrderManagement, OrdersHookConfig>
{
    /// <summary>
    /// Construct the OrdersStartupHook
    /// </summary>
    public OrdersStartupHook(IModuleManager moduleManager, ILogger<OrdersStartupHook> logger, IConfigManager configuration)
        : base(moduleManager, configuration, logger)
    {
        if (Config.Operations is not { Length: > 0 })
        {
            InitializationResult = FunctionResult.WithError("No operations defined");
        }
    }

    /// <inheritdoc />
    protected override async Task OnModuleStarted(IServerModule module, IOrderManagement facade)
    {
        var hasEntries = facade.GetOperations(o => true).Any();

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

    private static OperationCreationContext CreateOperationsContext(OrdersHookConfig.ImporterConfig operation)
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
