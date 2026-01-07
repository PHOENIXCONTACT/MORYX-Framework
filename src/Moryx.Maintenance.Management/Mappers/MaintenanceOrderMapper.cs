// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Management.Mappers;

public static class MaintenanceOrderMapper
{

    public static MaintenanceOrderModel ToDto(this MaintenanceOrder maintenanceOrder)
       => new(
           maintenanceOrder.Id,
           new ResourceModel(maintenanceOrder.Resource?.Id ?? 0, maintenanceOrder.Resource?.Name ?? string.Empty) ?? default!,
           maintenanceOrder.Description,
           maintenanceOrder?.Interval?.ToModel() ?? default!,
           [.. maintenanceOrder!.Instructions!],
           maintenanceOrder.Block,
           maintenanceOrder.IsActive,
           maintenanceOrder.Created,
           [.. maintenanceOrder.Acknowledgements],
           maintenanceOrder.MaintenanceStarted);
}
