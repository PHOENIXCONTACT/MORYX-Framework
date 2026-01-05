// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Modules;

namespace Moryx.Maintenance.Management.Components;

/// <summary>
/// Maintenance manager to handle resource maintenance
/// </summary>

internal interface IMaintenanceManager : IPlugin
{
    /// <summary>
    /// List of all maintenance orders
    /// </summary>
    IEnumerable<MaintenanceOrder> Orders { get; }

    /// <summary>
    /// List of all maintenance acknowledged
    /// </summary>
    IEnumerable<Acknowledgement> Acknowledgements { get; }

    /// <summary>
    /// Raised when when a maintenance order is created
    /// </summary>
    event EventHandler OrderAdded;

    /// <summary>
    /// Raised when when a maintenance order is updated
    /// </summary>
    event EventHandler OrderUpdated;

    /// <summary>
    /// Raised when when a maintenance order is acknowledged
    /// </summary>
    event EventHandler<MaintenanceOrder> OrderAcknowledged;

    /// <summary>
    /// Raised when when a maintenance order is sent to a resource
    /// </summary>
    event EventHandler OrdersSent;

    /// <summary>
    /// Raised when when a maintenance order is overdue
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOverdue;

    /// <summary>
    /// Raised when when a maintenance has started
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceStarted;

    /// <summary>
    /// Indicates if any <see cref="IMaintainableResource"/> has an overdue maintenance 
    /// </summary>
    bool HasOverdueMaintenance { get; }

    /// <summary>
    /// Adds a maintenance order
    /// </summary>
    /// <param name="model"></param>
    Task Add(MaintenanceOrder model);

    /// <summary>
    /// Acknowledges the maintenance with the given <paramref name="maintenanceOrderId"/> was performed
    /// </summary>
    /// <param name="maintenanceOrderId">Maintenance Order</param>
    /// <param name="data">Acknowledgement</param>
    Task Acknowledge(long maintenanceOrderId, Acknowledgement data);

    /// <summary>
    /// Updates the maintenance order
    /// </summary>
    /// <param name="model"></param>
    Task Update(MaintenanceOrder model);

    /// <summary>
    /// Deletes the given <paramref name="model"/>
    /// </summary>
    /// <param name="model"></param>
    Task Delete(MaintenanceOrder model);

    /// <summary>
    /// Start the maintenance for the given <paramref name="maintenanceOrderId"/>
    /// </summary>
    /// <param name="maintenanceOrderId">The maintenance order id</param>
    void Start(long maintenanceOrderId);
}
