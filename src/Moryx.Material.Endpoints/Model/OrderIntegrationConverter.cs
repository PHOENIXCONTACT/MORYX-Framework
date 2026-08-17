// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Integrations.Orders;
using Moryx.Tools;

namespace Moryx.Material.Endpoints.Model;

internal static class OrderIntegrationConverter
{
    #region ToModel

    public static OrderReferenceModel[] ToModels(this IReadOnlyList<OrderReference> references)
    {
        return references.Select(r =>
        {
            var status = r.Status;
            return new OrderReferenceModel()
            {
                OrderNumber = r.OrderNumber,
                OperationNumber = r.OperationNumber,
                StatusKey = status is null ? -1 : (int)status,
                StatusDisplayName = status?.GetDisplayName(),
                ReferenceStateKey = (int)r.State,
                ReferenceStateDisplayName = r.State.GetDisplayName(),
                OperationSourceType = r.Source?.GetType().GetDisplayName()
            };
        }).ToArray();
    }

    #endregion
}
