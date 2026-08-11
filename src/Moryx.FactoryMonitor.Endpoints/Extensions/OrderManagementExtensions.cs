// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.Orders;

namespace Moryx.FactoryMonitor.Endpoints.Extensions;

internal static class OrderManagementExtensions
{
    public static List<OrderModel> GetOrderModels(this IOrderManagement orderManager, string[] colorPalette)
    {
        var orderModels = orderManager.GetOperations(x => x.State is OperationStateClassification.Running)
                .Select(Converter.Converter.ToOrderModel).OrderBy(x => x.Order).ThenBy(x => x.Operation).ToList();

        // Assign color to order
        for (var i = 0; i < orderModels.Count; i++)
        {
            orderModels[i].Color = colorPalette[i % colorPalette.Length];
        }

        return orderModels;
    }
}
