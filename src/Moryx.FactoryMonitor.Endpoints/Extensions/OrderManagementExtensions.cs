// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.Orders;

namespace Moryx.FactoryMonitor.Endpoints.Extensions
{
    internal static class OrderManagementExtensions
    {
        public static List<OrderModel> GetOrderModels(this IOrderManagement orderManager, string[] colorPalette)
        {
            var orders = orderManager.GetOperations(x => x.State is OperationStateClassification.Running)
                .Select(Converter.Converter.ToOrderModel).ToList();

            // Assign color to order
            for (int i = 0; i < orders.Count; i++)
                orders[i].Color = colorPalette[i % colorPalette.Length];

            return orders;
        }
    }
}

