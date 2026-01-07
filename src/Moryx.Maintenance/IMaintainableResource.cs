// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Maintenance;

/// <summary>
/// A resource that can be maintained
/// </summary>
public interface IMaintainableResource : IResource
{
    /// <summary>
    /// Given a <paramref name="maintenance"/>, starts maintenance on the resource.
    /// </summary>
    /// <param name="maintenance">Order of the maintenance</param>
    void StartMaintenance(MaintenanceOrderStart maintenance);

    /// <summary>
    /// Raised when the resource performs any process.
    /// </summary>
    event EventHandler<int> CycleChanged;

    /// <summary>
    /// Raised when the resource has completed a maintenance
    /// </summary>
    event EventHandler<AcknowledgementEventArgs> MaintenanceCompleted;

    /// <summary>
    /// Raised when the resource has started the maintenance
    /// </summary>
    event EventHandler<MaintenanceEventArgs> MaintenanceStarted;
}
