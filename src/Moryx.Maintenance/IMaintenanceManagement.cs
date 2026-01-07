// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
namespace Moryx.Maintenance;

/// <summary>
/// Maintenance management to manage resource maintenance
/// </summary>
public interface IMaintenanceManagement
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
    Task AddMaintenanceOrderAsync(MaintenanceOrder model, CancellationToken cancellationToken);

    /// <summary>
    /// Acknowledges the maintenance with the given <paramref name="maintenanceOrderId"/> was performed
    /// </summary>
    /// <param name="maintenanceOrderId">Maintenance Order</param>
    /// <param name="data">Acknowledgement</param>
    Task AcknowledgeMaintenanceOrderAsync(long maintenanceOrderId, Acknowledgement data, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the maintenance order
    /// </summary>
    /// <param name="model"></param>
    Task UpdateMaintenanceOrderAsync(MaintenanceOrder model, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the given <paramref name="model"/>
    /// </summary>
    /// <param name="model"></param>
    Task DeleteMaintenanceOrderAsync(MaintenanceOrder model, CancellationToken cancellationToken);

    /// <summary>
    /// Start the maintenance for the given maintenance <paramref name="order"/>
    /// </summary>
    /// <param name="order">The maintenance order</param>
    Task StartMaintenanceAsync(MaintenanceOrder order, CancellationToken cancellationToken);

    /// <summary>
    /// Raised when when a maintenance order is created
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOrderAdded;

    /// <summary>
    /// Raised when when a maintenance order is updated
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOrderUpdated;

    /// <summary>
    /// Raised when when a maintenance order is acknowledged
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOrderAcknowledged;

    /// <summary>
    /// Raised when when a maintenance order is sent to a resource
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOrderSent;

    /// <summary>
    /// Raised when when a maintenance order is overdue
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOverdue;

    /// <summary>
    /// Raised when when a maintenance has Started
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceStarted;
}
