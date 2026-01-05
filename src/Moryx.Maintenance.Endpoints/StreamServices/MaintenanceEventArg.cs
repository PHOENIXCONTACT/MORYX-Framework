// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Maintenance.Management.Models;

namespace Moryx.Maintenance.Endpoints.StreamServices;

public class MaintenanceEventArg(MaintenanceOrderModel maintenanceOrder) : EventArgs
{
    public MaintenanceOrderModel MaintenanceOrder { get; } = maintenanceOrder;
}
