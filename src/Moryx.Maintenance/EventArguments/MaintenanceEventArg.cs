// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
namespace Moryx.Maintenance.EventArguments;

/// <summary>
/// Arguments for the Maintenance event
/// </summary>
public class MaintenanceEventArg(long maintenanceOrderId) : EventArgs
{
    /// <summary>
    /// OrderId of the Maintenance Order
    /// </summary>
    public long MaintenanceOrderId { get; } = maintenanceOrderId;
}
