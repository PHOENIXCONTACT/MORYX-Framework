// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.FactoryMonitor.Endpoints.Model;
using Moryx.Orders;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.FactoryMonitor.Endpoints.Extensions
{
    internal static class OrderManagementExtensions
    {
        public static List<OrderModel> GetOrderModels(this IOrderManagement orderManager, string[] colorPalette)
        {
            var orders = orderManager.GetOperations(x => x.State is OperationClassification.Running)
                .Select(Converter.ToOrderModel).ToList();

            // Assign color to order
            for (int i = 0; i < orders.Count; i++)
                orders[i].Color = colorPalette[i % colorPalette.Length];

            return orders;
        }
    }
}

