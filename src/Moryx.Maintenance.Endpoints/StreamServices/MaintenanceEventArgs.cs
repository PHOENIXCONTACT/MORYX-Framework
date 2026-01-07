// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Endpoints.StreamServices;

/// <summary>
/// Arguments for the Maintenance event
/// </summary>
/// <param name="maintenanceOrder"></param>
public class MaintenanceEventArgs(MaintenanceOrderModel maintenanceOrder) : EventArgs
{
    /// <summary>
    /// The maintenance order related to the event
    /// </summary>
    public MaintenanceOrderModel MaintenanceOrder { get; } = maintenanceOrder;
}
