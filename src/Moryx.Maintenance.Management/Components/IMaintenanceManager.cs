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
    IReadOnlyList<MaintenanceOrder> Orders { get; }

    /// <summary>
    /// List of all maintenance acknowledged
    /// </summary>
    IReadOnlyList<Acknowledgement> Acknowledgements { get; }

    /// <summary>
    /// Indicates if any <see cref="IMaintainableResource"/> has an overdue maintenance 
    /// </summary>
    bool HasOverdueMaintenance { get; }

    /// <summary>
    /// Adds a maintenance order
    /// </summary>
    /// <param name="model"></param>
    Task AddAsync(MaintenanceOrder model, CancellationToken cancellationToken);

    /// <summary>
    /// Acknowledges the maintenance with the given <paramref name="maintenanceOrderId"/> was performed
    /// </summary>
    /// <param name="maintenanceOrderId">Maintenance Order</param>
    /// <param name="data">Acknowledgement</param>
    Task AcknowledgeAsync(long maintenanceOrderId, Acknowledgement data, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the maintenance order
    /// </summary>
    /// <param name="model"></param>
    Task UpdateAsync(MaintenanceOrder model, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the given <paramref name="model"/>
    /// </summary>
    /// <param name="model"></param>
    Task DeleteAsync(MaintenanceOrder model, CancellationToken cancellationToken);

    /// <summary>
    /// Start the maintenance for the given <paramref name="maintenanceOrderId"/>
    /// </summary>
    /// <param name="maintenanceOrderId">The maintenance order id</param>
    Task StartAsync(long maintenanceOrderId, CancellationToken cancellationToken);

    /// <summary>
    /// Raised when a maintenance order is created
    /// </summary>
    event EventHandler<MaintenanceOrder> OrderAdded;

    /// <summary>
    /// Raised when a maintenance order is updated
    /// </summary>
    event EventHandler<MaintenanceOrder> OrderUpdated;

    /// <summary>
    /// Raised when a maintenance order is acknowledged
    /// </summary>
    event EventHandler<MaintenanceOrder> OrderAcknowledged;

    /// <summary>
    /// Raised when a maintenance order is sent to a resource
    /// </summary>
    event EventHandler<MaintenanceOrder> OrdersSent;

    /// <summary>
    /// Raised when a maintenance order is overdue
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOverdue;

    /// <summary>
    /// Raised when a maintenance has started
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceStarted;
}
