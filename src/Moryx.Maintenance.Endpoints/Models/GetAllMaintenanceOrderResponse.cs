// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Endpoints.Models;

/// <summary>
/// Response model for getting all maintenance orders
/// </summary>
/// <param name="Data"></param>
public record GetAllMaintenanceOrderResponse(List<MaintenanceOrderModel> Data);
