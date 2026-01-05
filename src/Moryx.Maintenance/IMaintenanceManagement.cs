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
    IEnumerable<MaintenanceOrder> Orders { get; }

    /// <summary>
    /// List of all maintenance acknowledged
    /// </summary>
    IEnumerable<Acknowledgement> Acknowledgements { get; }

    /// <summary>
    /// Raised when when a maintenance order is created
    /// </summary>
    event EventHandler MaintenanceOrderAdded;

    /// <summary>
    /// Raised when when a maintenance order is updated
    /// </summary>
    event EventHandler MaintenanceOrderUpdated;

    /// <summary>
    /// Raised when when a maintenance order is acknowledged
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOrderAcknowledged;

    /// <summary>
    /// Raised when when a maintenance order is sent to a resource
    /// </summary>
    event EventHandler MaintenanceOrderSent;

    /// <summary>
    /// Raised when when a maintenance order is overdue
    /// </summary>
    event EventHandler<MaintenanceOrder> MaintenanceOverdue;

    /// <summary>
    /// Raised when when a maintenance has Started
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
    void AddMaintenanceOrder(MaintenanceOrder model);

    /// <summary>
    /// Acknowledges the maintenance with the given <paramref name="maintenanceOrderId"/> was performed
    /// </summary>
    /// <param name="maintenanceOrderId">Maintenance Order</param>
    /// <param name="data">Acknowledgement</param>
    void AcknowledgeMaintenanceOrder(long maintenanceOrderId, Acknowledgement data);

    /// <summary>
    /// Updates the maintenance order
    /// </summary>
    /// <param name="model"></param>
    void UpdateMaintenanceOrder(MaintenanceOrder model);

    /// <summary>
    /// Deletes the given <paramref name="model"/>
    /// </summary>
    /// <param name="model"></param>
    void DeleteMaintenanceOrder(MaintenanceOrder model);

    /// <summary>
    /// Start the maintenance for the given maintenance <paramref name="order"/>
    /// </summary>
    /// <param name="order">The maintenance order</param>
    void StartMaintenance(MaintenanceOrder order);
}
