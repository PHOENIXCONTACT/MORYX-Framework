// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.AbstractionLayer.Resources;
using Moryx.Maintenance.EventArguments;

namespace Moryx.Maintenance;

/// <summary>
/// A resource that can be maintained
/// </summary>
public interface IMaintainableResource : IResource
{
    /// <summary>
    /// Raised when the resource performs any process.
    /// </summary>
    event EventHandler<int> CycleChanged;

    /// <summary>
    /// Raised when the resource has completed a maintenance
    /// </summary>
    event EventHandler<AcknowledgementEventArg> MaintenanceCompleted;

    /// <summary>
    /// Raised when the resource has started the maintenance
    /// </summary>
    event EventHandler<MaintenanceEventArg> MaintenanceStarted;

    /// <summary>
    /// Given a <paramref name="maintenance"/>, starts maintenance on the resource.
    /// </summary>
    /// <param name="maintenance">Order of the maintenance</param>
    void StartMaintenance(MaintenanceOrderStart maintenance);
}
