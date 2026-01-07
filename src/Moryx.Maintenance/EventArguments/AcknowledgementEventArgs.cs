// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Maintenance;

/// <summary>
/// Arguments of a maintenance that was acknowledge/done on a resource
/// </summary>
public class AcknowledgementEventArgs
{
    /// <summary>
    /// Operator that performed the maintenance
    /// </summary>
    public long OperatorId { get; set; }

    /// <summary>
    /// Description of the acknowledgement
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// OrderId of the Maintenance Order for which this acknowledgement is for
    /// </summary>
    public required long MaintenanceOrderId { get; set; }

}
